using System;

namespace ZebraUDP
{
    public class PrinterState
    {
        public event StateChangeEventHandler StateChangeHandler;
        public delegate void StateChangeEventHandler();

        public bool StateChangeFlag { get; set; }

        /// <summary>
        /// False = No response
        /// </summary>
        public bool Connected {
            get => _connected;
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    StateChangeHandler?.Invoke();
                }
            }
        }
        /// <summary>
        /// 缺紙
        /// </summary>
        public bool PaperOut
        {
            get => _paperOut;
            set
            {
                if (_paperOut != value)
                {
                    _paperOut = value;
                    StateChangeFlag = value;
                }
            }
        }
        /// <summary>
        /// 碳帶不足
        /// </summary>
        public bool RibbonOut
        {
            get => _ribbonOut;
            set
            {
                if (_ribbonOut != value)
                {
                    _ribbonOut = value;
                    StateChangeFlag = value;
                }
            }
        }
        /// <summary>
        /// 暫停狀態
        /// </summary>
        public bool Pause
        {
            get => _pause;
            set
            {
                if (_pause != value)
                {
                    _pause = value;
                    StateChangeFlag = value;
                }
            }
        }
        /// <summary>
        /// 列印錯誤
        /// </summary>
        public bool Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    StateChangeFlag = value;
                }
            }
        }
        /// <summary>
        /// Printer Warning!!
        /// </summary>
        public bool Warning
        {
            get => _warning;
            set
            {
                if (_warning != value)
                {
                    _warning = value;
                    StateChangeFlag = value;
                }
            }
        }
        /// <summary>
        /// Error Number
        /// </summary>
        public int ErrorNum
        {
            get => _errorNum;
            set
            {
                if (_errorNum != value)
                {
                    _errorNum = value;
                    StateChangeFlag = true;
                }
            }
        }
        /// <summary>
        /// Warning Number
        /// </summary>
        public int WarningNum
        {
            get => _warningNum;
            set
            {
                if (_warningNum != value)
                {
                    _warningNum = value;
                    StateChangeFlag = true;
                }
            }
        }
        /// <summary>
        /// Return Program Error
        /// </summary>
        public int SystemNum
        {
            get => _systemNum;
            set
            {
                if (_systemNum != value)
                {
                    _systemNum = value;
                    StateChangeFlag = true;
                }
            }
        }

        private bool _connected;
        private bool _paperOut;
        private bool _ribbonOut;
        private bool _pause;
        private bool _error;
        private bool _warning;
        private int _errorNum;
        private int _warningNum;
        private int _systemNum;

        public bool IsStateChangeHandlerNull()
        {
            return StateChangeHandler != null;
        }

        public void StateChange()
        {
            OnStateChange();
        }

        protected void OnStateChange()
        {
            StateChangeHandler?.Invoke();
        }
    }
}
