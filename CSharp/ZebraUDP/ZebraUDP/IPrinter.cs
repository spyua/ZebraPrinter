using System;
using System.Collections.Generic;
using System.Text;

namespace ZebraUDP
{
    public interface IPrinter
    {
        void Send(string code);

        void Receive(IAsyncResult ar);
    }
}
