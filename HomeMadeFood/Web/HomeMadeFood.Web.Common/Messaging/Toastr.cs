﻿using System;
using System.Collections.Generic;

namespace HomeMadeFood.Web.Common.Messaging
{
    [Serializable]
    public class Toastr
    {
        public Toastr()
        {
            this.ToastMessages = new List<ToastMessage>();
            this.ShowNewestOnTop = false;
            this.ShowCloseButton = false;
        }

        public bool ShowNewestOnTop { get; set; }

        public bool ShowCloseButton { get; set; }

        public ICollection<ToastMessage> ToastMessages { get; set; }

        public ToastMessage AddToastMessage(string title, string message, ToastType toastType)
        {
            var toast = new ToastMessage()
            {
                Title = title,
                Message = message,
                ToastType = toastType
            };
            ToastMessages.Add(toast);
            return toast;
        }
    }
}