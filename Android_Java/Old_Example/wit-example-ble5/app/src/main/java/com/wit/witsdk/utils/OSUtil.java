package com.wit.witsdk.utils;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Environment;
import android.text.TextUtils;
import android.util.Log;

import androidx.annotation.RequiresApi;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Enumeration;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.Set;

/**
 * 设备系统工具类
 * 作者： JairusTse
 * 日期： 17/12/19
 */

public class OSUtil {

    //MIUI标识
    private static final String KEY_MIUI_VERSION_CODE = "ro.miui.ui.version.code";
    private static final String KEY_MIUI_VERSION_NAME = "ro.miui.ui.version.name";
    private static final String KEY_MIUI_INTERNAL_STORAGE = "ro.miui.internal.storage";

    //EMUI标识
    private static final String KEY_EMUI_VERSION_CODE = "ro.build.version.emui";
    private static final String KEY_EMUI_API_LEVEL = "ro.build.hw_emui_api_level";
    private static final String KEY_EMUI_CONFIG_HW_SYS_VERSION = "ro.confg.hw_systemversion";

    //Flyme标识
    private static final String KEY_FLYME_ID_FALG_KEY = "ro.build.display.id";
    private static final String KEY_FLYME_ID_FALG_VALUE_KEYWORD = "Flyme";
    private static final String KEY_FLYME_ICON_FALG = "persist.sys.use.flyme.icon";
    private static final String KEY_FLYME_SETUP_FALG = "ro.meizu.setupwizard.flyme";
    private static final String KEY_FLYME_PUBLISH_FALG = "ro.flyme.published";

    /**
     * 检查权限
     *
     * @author huangyajun
     * @date 2022/11/5 18:12
     */
    public static void requestPermission(Activity context) {
        List<String> permissions = new ArrayList<>();
        int REQUEST_CODE_CONTACT = 101;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            permissions.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);
            permissions.add(Manifest.permission.READ_EXTERNAL_STORAGE);
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            permissions.add(Manifest.permission.MANAGE_EXTERNAL_STORAGE);
        }

        context.runOnUiThread(() -> {
            //验证是否许可权限
            for (String str : permissions) {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                    if (context.checkSelfPermission(str) != PackageManager.PERMISSION_GRANTED) {
                        //申请权限
                        context.requestPermissions(permissions.toArray(new String[0]), REQUEST_CODE_CONTACT);
                    }
                }
            }
        });

    }

    /**
     * 是否是Flyme系统
     *
     * @return
     */
    public static boolean isFlyme() {
        if (isPropertiesExist(KEY_FLYME_ICON_FALG, KEY_FLYME_SETUP_FALG, KEY_FLYME_PUBLISH_FALG)) {
            return true;
        }
        try {
            BuildProperties buildProperties = BuildProperties.newInstance();
            if (buildProperties.containsKey(KEY_FLYME_ID_FALG_KEY)) {
                String romName = buildProperties.getProperty(KEY_FLYME_ID_FALG_KEY);
                if (!TextUtils.isEmpty(romName) && romName.contains(KEY_FLYME_ID_FALG_VALUE_KEYWORD)) {
                    return true;
                }
            }
        } catch (IOException e) {
//            e.printStackTrace();
            Log.e("", "无法判断是不是 Flyme 系统");
        }
        return false;
    }

    /**
     * 是否是EMUI系统
     *
     * @return
     */
    public static boolean isEMUI() {
        return isPropertiesExist(KEY_EMUI_VERSION_CODE, KEY_EMUI_API_LEVEL,
                KEY_EMUI_CONFIG_HW_SYS_VERSION);
    }

    /**
     * 是否是MIUI系统
     *
     * @return
     */
    public static boolean isMIUI() {
        return isPropertiesExist(KEY_MIUI_VERSION_CODE, KEY_MIUI_VERSION_NAME,
                KEY_MIUI_INTERNAL_STORAGE);
    }

    private static boolean isPropertiesExist(String... keys) {
        if (keys == null || keys.length == 0) {
            return false;
        }
        try {
            BuildProperties properties = BuildProperties.newInstance();
            for (String key : keys) {
                String value = properties.getProperty(key);
                if (value != null)
                    return true;
            }
            return false;
        } catch (IOException e) {
            return false;
        }
    }

    private static final class BuildProperties {

        private final Properties properties;

        private BuildProperties() throws IOException {
            properties = new Properties();
            // 读取系统配置信息build.prop类
            properties.load(new FileInputStream(new File(Environment.getRootDirectory(), "build" +
                    ".prop")));
        }

        public boolean containsKey(final Object key) {
            return properties.containsKey(key);
        }

        public boolean containsValue(final Object value) {
            return properties.containsValue(value);
        }

        public Set<Map.Entry<Object, Object>> entrySet() {
            return properties.entrySet();
        }

        public String getProperty(final String name) {
            return properties.getProperty(name);
        }

        public String getProperty(final String name, final String defaultValue) {
            return properties.getProperty(name, defaultValue);
        }

        public boolean isEmpty() {
            return properties.isEmpty();
        }

        public Enumeration<Object> keys() {
            return properties.keys();
        }

        public Set<Object> keySet() {
            return properties.keySet();
        }

        public int size() {
            return properties.size();
        }

        public Collection<Object> values() {
            return properties.values();
        }

        public static BuildProperties newInstance() throws IOException {
            return new BuildProperties();
        }
    }
}

