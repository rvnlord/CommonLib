﻿using System;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class ActionStatus
    {
        public bool IsSuccess => ErrorCode == ErrorCode.Success;
        public bool IsFailure => !IsSuccess;
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }

        public ActionStatus(ErrorCode code, string message)
        {
            ErrorCode = code;
            Message = message;
        }

        public ActionStatus(string successMessage)
        {
            ErrorCode = ErrorCode.Success;
            Message = successMessage;
        }

        public static ActionStatus Success()
        {
            return new ActionStatus("Sukces");
        }

        public static bool operator !(ActionStatus actionStatus)
        {
            if (actionStatus == null)
                throw new ArgumentNullException(nameof(actionStatus));

            return actionStatus.IsFailure;
        }

        public bool LogicalNot()
        {
            return IsFailure;
        }
    }

    public enum ErrorCode
    {
        Success = 0,
        CannotSetClipboardText
    }
}
