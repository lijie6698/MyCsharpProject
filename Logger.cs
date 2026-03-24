using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepTrainingTest
{
    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// 日志条目类
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}";
        }
    }

    /// <summary>
    /// 静态日志类，提供日志记录功能
    /// </summary>
    public static class Logger
    {
        private static readonly List<LogEntry> _logs = new List<LogEntry>();
        private static readonly object _lock = new object();

        /// <summary>
        /// 获取所有日志条目的只读列表
        /// </summary>
        public static IReadOnlyList<LogEntry> Logs => _logs.AsReadOnly();

        /// <summary>
        /// 记录一条日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="level">日志级别，默认为Info</param>
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message
            };

            lock (_lock)
            {
                _logs.Add(entry);
            }

            // 触发日志添加事件
            LogAdded?.Invoke(null, new LogAddedEventArgs(entry));
        }

        /// <summary>
        /// 清除所有日志
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _logs.Clear();
            }

            LogCleared?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// 日志添加事件
        /// </summary>
        public static event EventHandler<LogAddedEventArgs> LogAdded;

        /// <summary>
        /// 日志清除事件
        /// </summary>
        public static event EventHandler LogCleared;
    }

    /// <summary>
    /// 日志添加事件参数
    /// </summary>
    public class LogAddedEventArgs : EventArgs
    {
        public LogEntry LogEntry { get; }

        public LogAddedEventArgs(LogEntry logEntry)
        {
            LogEntry = logEntry;
        }
    }
}