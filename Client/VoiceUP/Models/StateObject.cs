﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VoiceUP.Models
{
    class StateObject
    {
        public byte[] buffer = new byte[1024];
        public static int BufferSize = 1024;
        public StringBuilder sb = new StringBuilder();
        public Socket workSocket;
    }
}