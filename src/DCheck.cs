﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Linq
{
    public static class DCheck
    {
        // <summary>
        /// 验证指定值的断言<paramref name="assertion"/>是否为真，如果不为真，抛出指定消息<paramref name="message"/>的指定类型<typeparamref name="TException"/>异常
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="assertion">要验证的断言。</param>
        /// <param name="message">异常消息。</param>
        private static void Require<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
            {
                return;
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            TException exception = (TException)Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }
        /// <summary>
        /// 验证指定值的断言表达式是否为真，不为值抛出<see cref="Exception"/>异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        public static void Required<T>(T value, Func<T, bool> assertionFunc, string message)
        {
            if (assertionFunc == null)
            {
                throw new ArgumentNullException(nameof(assertionFunc));
            }
            Require<Exception>(assertionFunc(value), message);
        }
        /// <summary>
        /// 验证指定值的断言表达式是否为真，不为真抛出<typeparamref name="TException"/>异常
        /// </summary>
        /// <typeparam name="T">要判断的值的类型</typeparam>
        /// <typeparam name="TException">抛出的异常类型</typeparam>
        /// <param name="value">要判断的值</param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        public static void Required<T, TException>(T value, Func<T, bool> assertionFunc, string message) where TException : Exception
        {
            if (assertionFunc == null)
            {
                throw new ArgumentNullException("assertionFunc");
            }
            Require<TException>(assertionFunc(value), message);
        }

        /// <summary>
        /// 检查参数不能为空引用，否则抛出<see cref="ArgumentNullException"/>异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void NotNull<T>(T value, string paramName)
        {
            Require<ArgumentNullException>(value != null, string.Format("参数“{0}”不能为空引用。", paramName));
        }

        /// <summary>
        /// 检查字符串不能为空引用或空字符串，否则抛出<see cref="ArgumentNullException"/>异常或<see cref="ArgumentException"/>异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NotNullOrEmpty(string value, string paramName)
        {
            Require<ArgumentException>(!string.IsNullOrEmpty(value), string.Format("参数“{0}”不能为空引用或空字符串。", paramName));
        }

        /// <summary>
        /// 检查参数必须小于[或可等于，参数<paramref name="canEqual"/>]指定值，否则抛出<see cref="ArgumentOutOfRangeException"/>异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void LessThan<T>(T value, string paramName, T target, bool canEqual = false) where T : IComparable<T>
        {
            bool flag = canEqual ? value.CompareTo(target) <= 0 : value.CompareTo(target) < 0;
            string format = canEqual ? "参数“{0}”的值必须小于或等于“{1}”。" : "参数“{0}”的值必须小于“{1}”。";
            Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target));
        }

        /// <summary>
        /// 检查参数必须大于[或可等于，参数<paramref name="canEqual"/>]指定值，否则抛出<see cref="ArgumentOutOfRangeException"/>异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void GreaterThan<T>(T value, string paramName, T target, bool canEqual = false) where T : IComparable<T>
        {
            bool flag = canEqual ? value.CompareTo(target) >= 0 : value.CompareTo(target) > 0;
            string format = canEqual ? "参数“{0}”的值必须大于或等于“{1}”。" : "参数“{0}”的值必须大于“{1}”。";
            Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target));
        }

        /// <summary>
        /// 检查参数必须在指定范围之间，否则抛出<see cref="ArgumentOutOfRangeException"/>异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="start">比较范围的起始值。</param>
        /// <param name="end">比较范围的结束值。</param>
        /// <param name="startEqual">是否可等于起始值</param>
        /// <param name="endEqual">是否可等于结束值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Between<T>(T value, string paramName, T start, T end, bool startEqual = false, bool endEqual = false)
            where T : IComparable<T>
        {
            bool flag = startEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            string message = startEqual
                ? string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间。", paramName, start, end)
                : string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间，且不能等于“{3}”。", paramName, start, end, start);
            Require<ArgumentOutOfRangeException>(flag, message);

            flag = endEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            message = endEqual
                ? string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间。", paramName, start, end)
                : string.Format("参数“{0}”的值必须在“{1}”与“{2}”之间，且不能等于“{3}”。", paramName, start, end, end);
            Require<ArgumentOutOfRangeException>(flag, message);
        }
    }
}
