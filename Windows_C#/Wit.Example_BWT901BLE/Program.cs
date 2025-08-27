using System;
using System.IO.Ports;
using System.Threading;
using Wit.Bluetooth.WinBlue.Interface;
using Wit.Bluetooth.WinBlue.Utils;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant;
using Wit.SDK.Modular.WitSensorApi.Modular.BWT901BLE;

namespace SensorFusionSystem
{
    public class SensorFusionProgram
    {
        // Modbus RTU协议结构体（保留原有超声波部分）
        public struct ModbusRequest
        {
            public byte Address;
            public byte Function;
            public ushort RegisterAddr;
            public ushort RegisterCount;
            public ushort Crc;
        }

        public struct ModbusResponse
        {
            public byte Address;
            public byte Function;
            public byte ByteCount;
            public ushort Data;
            public ushort Crc;
        }

        // 共享状态变量
        static bool imuConnected = false;
        static bool exitFlag = false;
        static IWinBlueManager bluetoothManager;
        static SerialPort ultrasonicPort1;
        static SerialPort ultrasonicPort2;
        static Bwt901ble imuDevice;
        static bool isIndoor = false; // 默认设为false（室外）

        // 主入口
        public static void Main(string[] args)
        {
            Console.WriteLine("启动传感器融合系统...");

            // 初始化IMU蓝牙
            bluetoothManager = WinBlueFactory.GetInstance();
            bluetoothManager.OnDeviceFound += OnImuDeviceFound;

            // 初始化超声波串口（保留原有配置）
            ultrasonicPort1 = new SerialPort("COM6", 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            ultrasonicPort2 = new SerialPort("COM7", 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            try
            {
                // 打开超声波串口（原有逻辑）
                ultrasonicPort1.Open();
                ultrasonicPort2.Open();
                Console.WriteLine("超声波传感器已连接");

                // 启动IMU扫描
                bluetoothManager.StartScan();
                Console.WriteLine("正在扫描IMU设备...");

                // 启动超声波读取线程（保留原有逻辑）
                Thread ultrasonicThread = new Thread(() => ReadUltrasonicData());
                ultrasonicThread.IsBackground = true;
                ultrasonicThread.Start();

                // 主控制循环
                while (!exitFlag)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                    {
                        exitFlag = true;
                        Console.WriteLine("正在停止系统...");
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"系统错误: {ex.Message}");
            }
            finally
            {
                // 清理资源
                bluetoothManager.StopScan();
                if (ultrasonicPort1.IsOpen) ultrasonicPort1.Close();
                if (ultrasonicPort2.IsOpen) ultrasonicPort2.Close();
                imuDevice?.Close();
                Console.WriteLine("系统已安全关闭");
            }
        }

        //IMU设备发现回调（新增）
        static void OnImuDeviceFound(string mac, string name)
        {
            if (imuConnected || !name?.StartsWith("WT") == true) return;

            try
            {
                imuDevice = new Bwt901ble(mac, name);
                imuDevice.Open();

                if (!imuDevice.IsOpen())
                {
                    Console.WriteLine("IMU连接失败");
                    return;
                }

                imuConnected = true;
                bluetoothManager.StopScan();
                Console.WriteLine($"IMU已连接: {name} [{mac}]");

                // 启动IMU数据读取线程
                Thread imuThread = new Thread(ReadImuData);
                imuThread.IsBackground = true;
                imuThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IMU连接错误: {ex.Message}");
            }
        }

        // IMU数据读取（新增）
        static void ReadImuData()
        {
            try
            {
                while (!exitFlag && imuDevice.IsOpen())
                {
                    var accX = imuDevice.GetDeviceData(new DoubleKey("AccX", "X轴加速度", "m/s²"));
                    var accY = imuDevice.GetDeviceData(new DoubleKey("AccY", "Y轴加速度", "m/s²"));
                    var accZ = imuDevice.GetDeviceData(new DoubleKey("AccZ", "Z轴加速度", "m/s²"));
                    var asX = imuDevice.GetDeviceData(new DoubleKey("AsX", "X轴角速度", "°/s"));
                    var asY = imuDevice.GetDeviceData(new DoubleKey("AsY", "Y轴角速度", "°/s"));
                    var asZ = imuDevice.GetDeviceData(new DoubleKey("AsZ", "Z轴角速度", "°/s"));


                    Console.WriteLine($"IMU数据 - X:{accX?.ToString("F2")} Y:{accY?.ToString("F2")} Z:{accZ?.ToString("F2")} m/s²");
                    Console.WriteLine("\n");
                    Console.WriteLine($"IMU数据 - X:{asX?.ToString("F2")} Y:{asY?.ToString("F2")} Z:{asZ?.ToString("F2")} °/s");
                    Thread.Sleep(100);
                }
            }
            finally
            {
                imuDevice?.Close();
                Console.WriteLine("IMU连接已断开");
            }
        }

        //超声波数据读取（保留原有逻辑）
        private static ushort? ReadUltrasonicData(SerialPort serialPort)
        {
            // 准备Modbus请求帧 (读取处理值0x0100)
            byte[] request = new byte[8] {
            0x01,       // 设备地址
            0x03,       // 功能码(读取保持寄存器)
            0x01, 0x01, // 寄存器地址(0x0100)  处理值   01实时值
            0x00, 0x01, // 寄存器数量(1个)
            0x00, 0x00  // CRC占位(后面计算)
        };

            // 计算CRC并填充
            ushort crc = CalculateCRC(request, 6);
            request[6] = (byte)(crc & 0xFF);
            request[7] = (byte)((crc >> 8) & 0xFF);

            // 发送请求
            try
            {
                serialPort.Write(request, 0, request.Length);

                // 等待响应(根据文档，最大响应时间750ms)
                Thread.Sleep(800);

                // 读取响应
                byte[] response = new byte[7];
                int bytesRead = 0;
                while (serialPort.BytesToRead > 0 && bytesRead < response.Length)
                {
                    bytesRead += serialPort.Read(response, bytesRead, response.Length - bytesRead);
                }

                // 验证响应
                if (bytesRead != response.Length)
                {
                    Console.WriteLine("响应长度不正确");
                    return null;
                }

                // 检查CRC
                ushort receivedCRC = (ushort)((response[bytesRead - 1] << 8) | response[bytesRead - 2]);
                ushort calculatedCRC = CalculateCRC(response, bytesRead - 2);
                if (receivedCRC != calculatedCRC)
                {
                    Console.WriteLine("CRC校验失败");
                    return null;
                }

                // 解析数据
                return (ushort)((response[3] << 8) | response[4]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"通信错误: {ex.Message}");
                return null;
            }
        }

        static void ReadUltrasonicData()
        {
            try
            {
                while (!exitFlag)
                {
                    // 直接使用类成员变量
                    ushort? distance1 = ReadUltrasonicData(ultrasonicPort1);
                    ushort? distance2 = ReadUltrasonicData(ultrasonicPort2);

                    // 原有输出和报警逻辑保持不变
                    Console.WriteLine($"超声波数据 - 传感器1:{distance1?.ToString() ?? "无"}mm 传感器2:{distance2?.ToString() ?? "无"}mm");

                    if (distance1.HasValue && distance2.HasValue)
                    {
                        CheckObstacleAlarm(distance1.Value, distance2.Value,isIndoor);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"超声波读取错误: {ex.Message}");
            }
        }


        // 保留原有CRC计算
        static ushort CalculateCRC(byte[] buffer, int length)
        {
            ushort crc = 0xFFFF;
            for (int pos = 0; pos < length; pos++)
            {
                crc ^= buffer[pos];
                for (int i = 8; i != 0; i--)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        // 保留原有报警逻辑
        static void CheckObstacleAlarm(ushort a, ushort b, bool isIndoor)
        {
            // 定义室内外阈值（单位：mm）
            var thresholds = isIndoor ?
                (aSafe: 1200, aCritical: 800, bSafe: 1200) :  // 室内：安全1.2m，临界0.8m
                (aSafe: 2000, aCritical: 1600, bSafe: 2000);   // 室外：安全2m，临界1.6m

            // 安全状态判断
            if (a > thresholds.aSafe && b > thresholds.bSafe)
            {
                Console.WriteLine("无事发生");
            }
            // 脚踏板临界范围处理
            else if (a > thresholds.aCritical && a <= thresholds.aSafe)
            {
                if (isIndoor)
                {
                    Console.WriteLine("注意：前方接近障碍物！"); // 室内预警
                }
                else
                {
                    // 室外逻辑：左手把正常则报警
                    Console.WriteLine(b != 65533 ? "报警！！！" : "无事发生");
                }
            }
            // 紧急报警条件
            else if (a <= thresholds.aCritical || b <= thresholds.bSafe)
            {
                Console.WriteLine(isIndoor ? "报警！！！请立即减速" : "报警！！！");
            }

            Console.WriteLine("\n");
        }
    }
}