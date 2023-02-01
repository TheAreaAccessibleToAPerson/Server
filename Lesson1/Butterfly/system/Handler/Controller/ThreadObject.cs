using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Butterfly.system
{
    public abstract class ThreadObject : MainObject, IThreadSystem
    {
        #region Header

        private struct Data
        {
            public const int TIME_DELAY_IS_STOPPING_THREAD = 100;
            public const int TIME_DELAY_NO_START_PROCESS = 500;
        }

        private struct AccessData
        {
            public const string CREATING = "Creating";
            public const string RECORD = "Record";
            public const string ACCESS_IS_CLOSED = "AccessIsClosed";
        }

        private readonly SafeString ExternalAccessFlag = new SafeString(AccessData.CREATING);
        protected bool IsRecord { get { return ExternalAccessFlag.Comparison(AccessData.RECORD); } }

        public ThreadObject(string pTypeObject)
            : base(pTypeObject)
        {
            CancelTokenSource = new CancellationTokenSource();
            CancellationToken = CancelTokenSource.Token;
        }
        #endregion

        #region Threads

        private struct LifeData
        {
            public const string SPACER = ":";

            public const string STARTING = SPACER + "Starting";
            public const string STOPPING = SPACER + "Stopping";
            public const string THREAD = SPACER + "Thread";

            public const string START_THREADS = SPACER + "Start Threads.";
            public const string STOP_THREADS = SPACER + "Stop Threads.";
            public const string ADD = SPACER + "ADD";
            public const string START = SPACER + SPACER + "START";
            public const string STOP = SPACER + "STOP";
        }

        private readonly List<ThreadBuffer> ThreadBufferList = new List<ThreadBuffer>();

        private CancellationTokenSource CancelTokenSource;
        private CancellationToken CancellationToken;


        private int StartThread()
        {
            if (__StartProcess)
            {
                if (ThreadBufferList.Count == 0) return 0;
                
                SystemInformation($"{LifeData.STARTING} {ThreadBufferList.Count} {LifeData.THREAD}");

                CancelTokenSource = new CancellationTokenSource();
                CancellationToken = CancelTokenSource.Token;

                foreach (ThreadBuffer threadBuffer in ThreadBufferList)
                {
                    try
                    {
                        threadBuffer.Thread.Start();

                        SystemInformation(threadBuffer.Name + LifeData.START);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Exception(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Exception(ex);
                    }
                }

                SystemInformation(LifeData.START_THREADS);
            }

            return ThreadBufferList.Count;
        }

        private void StopThread()
        {
            if (ThreadBufferList.Count == 0) return;

            SystemInformation($"{LifeData.STOPPING} {ThreadBufferList.Count} {LifeData.THREAD}");

            CancelTokenSource.Cancel();

            foreach (ThreadBuffer threadBuffer in ThreadBufferList)
            {
                threadBuffer.BreakTheBlock();
            }

            Thread.Sleep(Data.TIME_DELAY_IS_STOPPING_THREAD);

            while (true)
            {
                int i = 0;
                foreach (ThreadBuffer threadBuffer in ThreadBufferList)
                {
                    if (threadBuffer.Thread.IsCompleted) i++;
                }

                if (i == ThreadBufferList.Count) break;

                Thread.Sleep(Data.TIME_DELAY_IS_STOPPING_THREAD);
            }

            SystemInformation(LifeData.STOP_THREADS);

            foreach (ThreadBuffer threadBuffer in ThreadBufferList) threadBuffer.Dispose();

            ThreadBufferList.Clear();
        }

        protected void AddThread(Action action, string pName, int pThreadTimeDelay, int pCountThread = 1)
        {
            if (!__IsStarting)
            {
                Exception(Ex.THRD.x400010);
                return;
            }
            else
            {
                CreatingThread(action, pName, pThreadTimeDelay, pCountThread);
            }
        }
        protected void AddThread(Action action, string pName, int pThreadTimeDelay, int pCountThread, Action pBreakTheBlockThreadAction)
        {
            if (!__IsStarting)
            {
                Exception(Ex.THRD.x400010);
                return;
            }
            else
            {
                CreatingThread(action, pName, pThreadTimeDelay, pCountThread, pBreakTheBlockThreadAction);
            }
        }

        void IThreadSystem.AddThread(System.Action action, string pName, int pThreadTimeDelay, int pCountThread)
        {
            CreatingThread(action, pName, pThreadTimeDelay, pCountThread);
        }

        private void CreatingThread(Action action, string pName, int pThreadTimeDelay, int pCountThread, Action pBreakTheBlockThreadAction = null)
        {
            for (int i = 0; i < pCountThread; i++)
            {
                Task task = new Task(() =>
                {
                    while (true)
                    {
                        if (CancellationToken.IsCancellationRequested)
                        {
                            SystemInformation(LifeData.THREAD + ":" + pName + LifeData.STOP);

                            return;
                        }
                        else
                        {
                            if (StartProcess)
                            {
                                action();

                                Thread.Sleep(pThreadTimeDelay);
                            }
                            else
                            {
                                Thread.Sleep(Data.TIME_DELAY_NO_START_PROCESS);
                            }
                        }
                    }
                }, CancellationToken);

                ThreadBufferList.Add(new ThreadBuffer(pName, task, pBreakTheBlockThreadAction));
                SystemInformation(pName + LifeData.ADD);
            }
        }

        #endregion

        #region SafeMainObject

        private readonly object MainObjectLocker = new object();

        #region System

        protected override void StartObj()
        {
            lock(MainObjectLocker)
            {
                base.StartObj();

                if (__StartProcess)
                {
                    StartThread();

                    ExternalAccessFlag.Set(AccessData.RECORD);
                }
            }
        }
        protected override void StopObj()
        {
            lock(MainObjectLocker)
            {
                ExternalAccessFlag.Set(AccessData.ACCESS_IS_CLOSED);

                StopThread();

                base.StopObj(); 
            }
        }

        #endregion

        #region Controller

        protected sealed override void AddObject(MainObject pControllerObject)
        {
            if (__IsStarting || __StartProcess)
            {
                lock (MainObjectLocker)
                {
                    base.AddObject(pControllerObject);
                }
            }
        }
        protected sealed override void AddObject(string pKey, MainObject pControllerObject)
        {
            if (__IsStarting || __StartProcess)
            {
                lock (MainObjectLocker)
                {
                    base.AddObject(pKey, pControllerObject);
                }
            }
        }
        protected override bool ContainsKeyObject(string pKey)
        {
            lock(MainObjectLocker)
            {
                return base.ContainsKeyObject(pKey);
            }
        }
        protected sealed override void SendToChildren<BufferType>(string pKey, BufferType pBuffer)
        {
            if (__IsStarting || __StartProcess)
            {
                lock (MainObjectLocker)
                {
                    base.SendToChildren(pKey, pBuffer);
                }
            }                
        }
        protected sealed override void SendToChildrenListener<ControllerType, BufferType>(string pKey, BufferType pBuffer)
        {
            lock(MainObjectLocker)
            {
                base.SendToChildrenListener<ControllerType, BufferType>(pKey, pBuffer);
            }
        }
        protected sealed override bool TryRemoveObject(string pKey)
        {
            lock(MainObjectLocker)
            {
                return base.TryRemoveObject(pKey);
            } 
        }
        protected sealed override void RemoveObject(string pKey)
        {
            lock (MainObjectLocker)
            {
                base.RemoveObject(pKey);
            }
        }

        #endregion

        #endregion
    }
}
