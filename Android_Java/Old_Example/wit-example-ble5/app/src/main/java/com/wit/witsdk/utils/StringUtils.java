package com.wit.witsdk.utils;

/**
 * 字符串操作工具类
 *
 * @author huangyajun
 * @date 2022/6/17 19:18
 */
public class StringUtils {

    /**
     * 字符串判为不空
     *
     * @author huangyajun
     * @date 2022/4/27 14:34
     */
    public static boolean isNotBlank(final CharSequence cs) {
        return !isBlank(cs);
    }

    /**
     * 字符串判空
     *
     * @author huangyajun
     * @date 2022/4/27 14:34
     */
    public static boolean isBlank(final CharSequence cs) {
        int strLen;
        if (cs == null || (strLen = cs.length()) == 0) {
            return true;
        }
        for (int i = 0; i < strLen; i++) {
            if (Character.isWhitespace(cs.charAt(i)) == false) {
                return false;
            }
        }
        return true;
    }

    /**
     * 判断字符串是null或者空串
     *
     * @author huangyajun
     * @date 2022/4/27 14:42
     */
    public static boolean IsNullOrEmpty(String str) {
        return str == null || str.equals("");
    }

    /**
     * String左对齐
     *
     * @author huangyajun
     * @date 2022/4/27 14:33
     */
    public static String padLeft(String src, int len, char ch) {

        int diff = len - src.length();
        if (diff <= 0) {
            return src;
        }

        char[] charr = new char[len];
        System.arraycopy(src.toCharArray(), 0, charr, diff, src.length());
        for (int i = 0; i < diff; i++) {
            charr[i] = ch;
        }
        return new String(charr);
    }

    /**
     * String右对齐
     *
     * @author huangyajun
     * @date 2022/4/27 14:33
     */
    public static String padRight(String src, int len, char ch) {
        int diff = len - src.length();
        if (diff <= 0) {
            return src;
        }

        char[] charr = new char[len];
        System.arraycopy(src.toCharArray(), 0, charr, 0, src.length());
        for (int i = src.length(); i < len; i++) {
            charr[i] = ch;
        }
        return new String(charr);

    }
}