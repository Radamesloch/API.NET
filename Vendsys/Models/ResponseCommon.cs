﻿namespace Vendsys.Models
{
    public class ResponseCommon
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
