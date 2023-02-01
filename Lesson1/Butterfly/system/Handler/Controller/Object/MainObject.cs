using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Butterfly.system
{
    public abstract class MainObject : IMainObject
    {
        #region Header

        /***********************************************************
         * Заголовок обьекта. Сдесь хранится информация об обьекте.
        ***********************************************************/

        protected struct TypeObjectData
        {
            public const string HANDLER = "Handler";
            public const string PROGRAM_CONTROLLER = "Program";
            public const string CONTROLLER = "Controller";
            public const string INDEPENDENT_CONTROLLER = "Controller.Independent";
        }

        public MainObject(string pTypeObject)
        {
            string[] type = pTypeObject.Split('.');
            if (type.Length == 1)
            {
                TypeObject = type[0];

                Independent = false;
            }
            else if (type.Length == 2)
            {
                TypeObject = type[0];
                if (type[1] == "Independent" || type[1] == TypeObjectData.PROGRAM_CONTROLLER) Independent = true;
            }

            Namespace = GetType().ToString();

            string[] namespaceSplit = Namespace.Split('.', 2);
            if (namespaceSplit[0] == Global.NAMESPACE && namespaceSplit.Length == 2)
            {
                NameObject = namespaceSplit[1];

                Explorer = $"[{TypeObject}]{NameObject}";
            }
            else
                Exception(Ex.MOBJ.x100029);
        }

        // Тип обьекта Program, Controler, Handler.
        private readonly string TypeObject;
        // Имя обьекта, это имя старшего класса.
        private readonly string NameObject;
        // Является тип индивидуальным или подконтрольным. Это значение необходимо
        // при остановки работы обьекта. Если обьект индивидуальный, то мы просто остановим его.
        // Если же подконтрольный, то нужно поднятся по древу вверх до родителя который является
        // индивидуальным и остановить его работу, а он уже в стою очередь остановит данный обьект.
        private readonly bool Independent;
        // Пространсво в котором находится обьект.
        private readonly string Namespace;
        // Данная строка будет использоваться для вывода сообщений.
        private readonly string Explorer;

        protected bool IsHandler { get { return TypeObject == TypeObjectData.HANDLER; } }
        protected bool IsController { get { return TypeObject == TypeObjectData.CONTROLLER; } }
        protected bool IsProgram { get { return TypeObject == TypeObjectData.PROGRAM_CONTROLLER; } }

        public string GetNameObject() => NameObject;
        public string GetTypeObject() => TypeObject;
        public string GetExplorerObject()
        {
            if (Parent == null)
            {
                return Explorer;
            }
            else
            {
                var p = "/"; int i = 0; string count = "";

                while ((i = Explorer.IndexOf(p, i)) != -1) { count += "  "; i += p.Length; }
                string fullExplorer = Explorer.Insert(0, Parent.GetExplorerObject() + "/");

                return $"{count}{fullExplorer}";
            }
        }
        public bool IsTypeObject(string pType) => TypeObject == pType;

        #endregion

        #region DOM

        private ProgramObject Program;
        private MainObject Parent;

        public string KeyObject { private set; get; }
        public int NodeNumber { private set; get; }

        void IDOM.CreatingNode()
        {
            ((IStateObject)this).Construction();

            ((IStateObject)this).Dependency();

            foreach (var pair in PrivateHandlers)
                ((IStateObject)pair.Value).Dependency();

            foreach (var pair in PublicHandlers)
                ((IStateObject)pair.Value).Dependency();

            foreach (var pair in ObjectsController)
                ((IStateObject)pair.Value).Dependency();

            StartObj();
        }

        void IDOM.DestroyNode()
        {
            Program.AddNodeToDestroy(() => 
            {
                if (Independent)
                {
                    ((IDOM)Parent).RemoveChildren(KeyObject);

                    StopObj();
                }
                else if (NodeNumber == 0)
                {
                    System.Threading.Tasks.Task.Run(() => StopObj());
                }
                else
                {
                    ((IDOM)Parent).DestroyNode();
                }
            });
        }

        void IDOM.RemoveChildren(string pKey)
        {
            TryRemoveObject(pKey);
        }


        void IDOM.SetDOM(ProgramObject pProgramObject, MainObject pParentObject, string pKeyObject, int pNodeNumber)
        {
            Program = pProgramObject;
            Parent = pParentObject;
            KeyObject = pKeyObject;
            NodeNumber = pNodeNumber;
        }

        System.Func<PrivateHandlerType> IDOM.AddPrivateHandler<PrivateHandlerType>()
        {
            if (Parent is IDependency.IObject parentDependencyObject)
            {
                return parentDependencyObject.AddPrivateHandler<PrivateHandlerType>();
            }

            return default;
        }
        System.Func<PrivateHandlerType> IDOM.AddPrivateHandler<PrivateHandlerType>(object pLocalBuffer)
        {
            if (Parent is IDependency.IObject parentDependencyObject)
            {
                return parentDependencyObject.AddPrivateHandler<PrivateHandlerType>(pLocalBuffer);
            }

            return default;
        }
        System.Func<PrivateHandlerType> IDOM.AddPrivateHandler<PrivateHandlerType>(object[] pLocalBuffers)
        {
            if (Parent is IDependency.IObject parentDependencyObject)
            {
                return parentDependencyObject.AddPrivateHandler<PrivateHandlerType>(pLocalBuffers);
            }

            return default;
        }

        bool IDOM.SendToHandler<HandlerType, BufferType>(BufferType pMessage)
        {
            if (GetHandler<HandlerType>() is IStream handlerReduse)
            {
                handlerReduse.ToInput(pMessage);

                return true;
            }

            return false;
        }
        bool IDOM.SendToChildrenListener<HandlerType, BufferType>(BufferType pMessage)
        {
            if (TryGetHandler(out HandlerType oHandler))
            {
                if (oHandler is IStream handlerReduse)
                {
                    handlerReduse.ToInput(pMessage);
                }

                return true;
            }
            else
            {
                foreach(var controller in ObjectsController)
                {
                    if(((IDOM)controller.Value).SendToChildrenListener<HandlerType, BufferType>(pMessage))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        bool IDOM.SendToParentListener<HandlerType, BufferType>(BufferType pMessage)
        {
            if (TryGetHandler(out HandlerType oHandler))
            {
                if (oHandler is IStream handlerReduse)
                {
                    handlerReduse.ToInput(pMessage);
                }

                return true;
            }
            else
            {
                if (NodeNumber > 0)
                {
                    return ((IDOM)Parent).SendToParentListener<HandlerType, BufferType>(pMessage);
                }      
            }

            return false;
        }
        void IDOM.SendToParent<BufferType>(BufferType pMessage)
        {

        }

        #endregion

        #region Controllers

        private readonly Dictionary<string, MainObject> ObjectsController = new Dictionary<string, MainObject>();
        private long ControllerKeyIndex = 777777;

        protected virtual void AddObject(MainObject pControllerObject)
        {
            if (__IsStarting || __StartProcess)
            {
                if (pControllerObject.GetTypeObject() == TypeObjectData.CONTROLLER)
                {
                    ObjectsController.Add(ControllerKeyIndex++.ToString(), pControllerObject);

                    ((IDOM)pControllerObject).SetDOM(Program, this, ControllerKeyIndex.ToString(), NodeNumber + 1);
                    ((IDOM)pControllerObject).CreatingNode();
                }
                else
                    Exception(Ex.MOBJ.x100002, pControllerObject.GetType().ToString());
            }
            else
                Exception(Ex.MOBJ.x100033, pControllerObject.GetType().ToString());
            
        }
        protected virtual void AddObject(string pKey, MainObject pControllerObject)
        {
            if (__IsStarting || __StartProcess)
            {
                if (ObjectsController.TryGetValue(pKey, out MainObject oMainObject))
                {
                    ObjectsController.Remove(pKey);

                    ((IDOM)oMainObject).DestroyNode();
                }
                else
                {
                    if (pControllerObject.GetTypeObject() == TypeObjectData.CONTROLLER)
                    {
                        ObjectsController.Add(pKey, pControllerObject);

                        ((IDOM)pControllerObject).SetDOM(Program, this, pKey, NodeNumber + 1);
                        ((IDOM)pControllerObject).CreatingNode();
                    }
                    else
                        Exception(Ex.MOBJ.x100002, pControllerObject.GetType().ToString());
                }
            }
            else
                Exception(Ex.MOBJ.x100033, pControllerObject.GetType().ToString());
        }
        protected void AddObject<ControllerObjectType>() where ControllerObjectType : new()
        {
            if (new ControllerObjectType() is MainObject controllerObjectReduse)
            {
                AddObject(controllerObjectReduse);
            }
            else
                Exception(Ex.MOBJ.x100003, new ControllerObjectType().GetType().ToString());
        }
        protected void AddObject<ControllerObjectType>(string pKey)
            where ControllerObjectType : new()
        {
            if (new ControllerObjectType() is MainObject controllerObjectReduse)
            {
                AddObject(pKey, controllerObjectReduse);
            }
            else
                Exception(Ex.MOBJ.x100003, new ControllerObjectType().GetType().ToString());
        }
        protected void AddObject(MainObject pControllerObject, Object pLocalBuffer)
        {
            if (pControllerObject is ILocalBuffer controllerObjectLocalBufferReduse)
            {
                controllerObjectLocalBufferReduse.SetBuffer(pLocalBuffer);
                AddObject(pControllerObject);
            }
            else
                Exception(Ex.MOBJ.x100012);
        }
        protected void AddObject(MainObject pControllerObject, Object[] pLocalBuffers)
        {
            if (pControllerObject is ILocalBuffer controllerObjectLocalBufferReduse)
            {
                controllerObjectLocalBufferReduse.SetBuffer(pLocalBuffers);
                AddObject(pControllerObject);
            }
            else
                Exception(Ex.MOBJ.x100013);
        }
        protected void AddObject(string pKey, MainObject pControllerObject, Object pLocalBuffer)
        {
            if (pControllerObject is ILocalBuffer controllerObjectLocalBufferReduse)
            {
                controllerObjectLocalBufferReduse.SetBuffer(pLocalBuffer);
                AddObject(pKey, pControllerObject);
            }
            else
                Exception(Ex.MOBJ.x100012);
        }
        protected void AddObject(string pKey, MainObject pControllerObject, Object[] pLocalBuffers)
        {
            if (pControllerObject is ILocalBuffers controllerObjectLocalBufferReduse)
            {
                controllerObjectLocalBufferReduse.SetBuffers(pLocalBuffers);
                AddObject(pKey, pControllerObject);
            }
            else
                Exception(Ex.MOBJ.x100013);
        }
        protected void AddObject<ControllerObjectType>(Object pLocalBuffer) where ControllerObjectType : new()
        {
            if (new ControllerObjectType() is MainObject controllerObjectReduse)
            {
                if (controllerObjectReduse is ILocalBuffer controllerObjectLocalBufferReduse)
                {
                    controllerObjectLocalBufferReduse.SetBuffer(pLocalBuffer); ;
                    AddObject(controllerObjectReduse);
                }
                else
                    Exception(Ex.MOBJ.x100013);
            }
            else
                Exception(Ex.MOBJ.x100003, new ControllerObjectType().GetType().ToString());
        }
        protected void AddObject<ControllerObjectType>(Object[] pLocalBuffers) where ControllerObjectType : new()
        {
            if (new ControllerObjectType() is MainObject controllerObjectReduse)
            {
                if (controllerObjectReduse is ILocalBuffers controllerObjectLocalBufferReduse)
                {
                    controllerObjectLocalBufferReduse.SetBuffers(pLocalBuffers);
                    AddObject(controllerObjectReduse);
                }
                else
                    Exception(Ex.MOBJ.x100013);
            }
            else
                Exception(Ex.MOBJ.x100003, new ControllerObjectType().GetType().ToString());
        }
        protected void AddObject<ControllerObjectType>(string pKey, Object pLocalBuffer) where ControllerObjectType : new()
        {
            if (new ControllerObjectType() is MainObject controllerObjectReduse)
            {
                if (controllerObjectReduse is ILocalBuffer controllerObjectLocalBufferReduse)
                {
                    controllerObjectLocalBufferReduse.SetBuffer(pLocalBuffer);
                    AddObject(pKey, controllerObjectReduse);
                }
                else
                    Exception(Ex.MOBJ.x100013);
            }
            else
                Exception(Ex.MOBJ.x100003, new ControllerObjectType().GetType().ToString());
        }
        protected void AddObject<ControllerObjectType>(string pKey, Object[] pLocalBuffers) where ControllerObjectType : new()
        {
            if (new ControllerObjectType() is MainObject controllerObjectReduse)
            {
                if (controllerObjectReduse is ILocalBuffers controllerObjectLocalBufferReduse)
                {
                    controllerObjectLocalBufferReduse.SetBuffers(pLocalBuffers);
                    AddObject(pKey, controllerObjectReduse);
                }
                else
                    Exception(Ex.MOBJ.x100013);
            }
            else
                Exception(Ex.MOBJ.x100003, new ControllerObjectType().GetType().ToString());
        }
        protected bool TryGetObject<ControllerObjectType>(string pKey, out ControllerObjectType oMainObject)
            where ControllerObjectType : new()
        {
            oMainObject = new ControllerObjectType();

            if (oMainObject is MainObject mainObjectReduse)
            {
                return ObjectsController.TryGetValue(pKey, out mainObjectReduse);
            }
            else
                return false;
        }
        protected bool TryGetObject(string pKey, out MainObject oMainObject)
        {
            return ObjectsController.TryGetValue(pKey, out oMainObject);
        }
        protected virtual bool ContainsKeyObject(string pKey)
        {
            return ObjectsController.ContainsKey(pKey);
        }
        protected virtual MainObject GetObject(string pKey)
        {
            if (ObjectsController.TryGetValue(pKey, out MainObject pControllerObject))
            {
                return pControllerObject;
            }
            else
                Exception(Ex.MOBJ.x100004);

            return default;
        }
        protected virtual bool TryRemoveObject(string pKey)
        {
            bool result = true;
            {
                if (ObjectsController.TryGetValue(pKey, out MainObject oControllerObject))
                {
                    ((IStateObject)oControllerObject).Stop();
                    ObjectsController.Remove(pKey);
                }
                else
                {
                    result = false;
                }

            }
            return result;
        }
        protected virtual void RemoveObject(string pKey)
        {
            if (ObjectsController.TryGetValue(pKey, out MainObject oControllerObject))
            {
                ((IStateObject)oControllerObject).Stop();
                ObjectsController.Remove(pKey);
            }
            else
                Exception(Ex.MOBJ.x100034, pKey);
        }
        protected virtual void RemoveFirstObject()
        {
            var objControllerDictionaryEnumerator = ObjectsController.GetEnumerator();
            objControllerDictionaryEnumerator.MoveNext();
            var keyFirstObject = objControllerDictionaryEnumerator.Current.Key;
            TryRemoveObject(keyFirstObject);
        }

        /// <summary>
        /// Высылает сообещние в обработчик Echo<BufferType>.Parent ребенка.
        /// </summary>
        protected virtual void SendToChildren<BufferType>(string pKey, BufferType pBuffer) where BufferType : struct
        {
            if (__IsCreating)
            {
                Exception(Ex.MOBJ.x100041);
            }
            else
            {
                if (TryGetObject(pKey, out MainObject oControllerObject))
                {
                    if (((IDOM)oControllerObject).SendToHandler<Echo<BufferType>.Parent, BufferType>(pBuffer))
                    {
                        //...
                    }
                    else
                        Exception(Ex.MOBJ.x100041, typeof(Echo<BufferType>.Parent).ToString());
                }
            }   
        }
        protected virtual void SendToParent<BufferType>(BufferType pBuffer) where BufferType : struct
        {
            if (__IsCreating)
            {
                Exception(Ex.MOBJ.x100041);
            }
            else
            {
                if (NodeNumber > 0)
                {
                    if (((IDOM)Parent).SendToHandler<Echo<BufferType>.Children, BufferType>(pBuffer))
                    {
                        //...
                    }
                    else
                        Exception(Ex.MOBJ.x100040, typeof(Echo<BufferType>.Children).ToString());
                }
                else
                    Exception(Ex.MOBJ.x100039);
            }    
        }
        /// <summary>
        /// Вводим сообщение в древо ребенка до первого слушателя.
        /// </summary>
        protected virtual void SendToChildrenListener<CurrentObjectType, BufferType>(string pKey, BufferType pBuffer) 
            where CurrentObjectType : class where BufferType : struct 
        {
            if (__IsCreating)
            {
                Exception(Ex.MOBJ.x100041);
            }
            else
            {
                if (TryGetObject(pKey, out MainObject oControllerObject))
                {
                    ((IDOM)oControllerObject).SendToChildrenListener<Echo<BufferType>.Parent<CurrentObjectType>, BufferType>(pBuffer);
                }
            }
        }

        protected virtual void SendToParentListener<CurrentObjectType, BufferType>(BufferType pBuffer)
            where CurrentObjectType : class where BufferType : struct
        {
            if (__IsCreating)
            {
                Exception(Ex.MOBJ.x100041);
            }
            else 
            {
                if (NodeNumber > 0)
                {
                    if (((IDOM)Parent).SendToParentListener<Echo<BufferType>.Children<CurrentObjectType>, BufferType>(pBuffer))
                    {
                        //...
                    }
                    else
                        Exception(Ex.MOBJ.x100037, ((IBuffer)new BufferType()).GetName(), GetNameObject());
                }
                else
                    Exception(Ex.MOBJ.x100036);
            }                
        }

        #endregion

        #region PublicHandlers

        /*******************************************************************************************
         * 
         * Публичные обработчики. Создать можно только в методе Contruction(). 
         * Доступ к ним можно получить во время работы программы.
         * В нутри реализованы все возможности Controller обьектов. Имеют входной поток
         * и выходной(Сквозной), доступ к которым можно получить из нутри обьекта с помощью
         * методов ToOutput() и TryGetInput(). Перенаправить поток можно с помощью небезопасного метода 
         * redirect_to() или с помощью безопасного аналогичного метода lock_redirect_to().
         * В качесве параметра можно передать:
         *  1)Action<BufferType> при первом вызове.
         *  2)Action<BufferType>(Action<BufferType>) c явным указанием входного типа тогда поток продублиреут 
         *      передачу Buffer до того как передастся в следующий обьект реализующий интерфейс IStream. 
         *  3)В публичный обьект обработчик указав в параметре .redirect_to(GetHandler<PublicHandlerType>) 
         *    или явно указав ключ .redirect_to .redirect_to(string pPublicHandlerKey).
         *  4)В приватный обьект обработик просто указав его тип .redirect_to<PrivateHandlerType>().
         *  5)В анализатор .redirect_to<AnalizatorType<InputBufferType, OutputBufferType>> который примит выходной 
         *    тип буфера у текущего обьетка реализуещего интерфейс IStream произведет над ним работу в абстрактном 
         *    методe Analiz(InputBufferType pBuffer) и далее передаст следующему .redirect_to().
         *  6)При отправке буфера в Func<InputBufferType, OutputBufferType> нужно явно указать входной тип буфера
         *    и выходной.
         * 
         *******************************************************************************************/

        private readonly Dictionary<string, object> PublicHandlers = new Dictionary<string, object>();
        private int PublicHandlerKeyIndex = 777777;

        private bool InstantiatePublicHandler<HandlerType>(HandlerType pHandler,
            out HandlerType oReturnHandler, string pKey = "") where HandlerType : new()
        {
            oReturnHandler = default;

            bool result = true;
            {
                if (!__IsCreating)
                {
                    return Exception(Ex.MOBJ.x100011);
                }
                else if (pHandler is MainObject controllerObjectReduse)
                {
                    if (controllerObjectReduse.GetTypeObject() == TypeObjectData.HANDLER)
                    {
                        string key = pKey;
                        if (key == "") key = PublicHandlerKeyIndex++.ToString();

                        if (PublicHandlers.TryGetValue(key, out Object handlerObject))
                        {
                            result = Exception(Ex.MOBJ.x100008);
                        }
                        else
                        {
                            ((IDOM)pHandler).SetDOM(Program, this, key, NodeNumber + 1);

                            PublicHandlers.Add(key, pHandler);

                            oReturnHandler = pHandler;
                        }
                    }
                    else result = Exception(Ex.MOBJ.x100007);
                }
                else result = Exception(Ex.MOBJ.x100006);
            }

            return result;
        }
        protected HandlerType AddHandler<HandlerType>() where HandlerType : new()
        {
            if (InstantiatePublicHandler(new HandlerType(), out HandlerType handlerResult))
            {
                return handlerResult;
            }
            else
                Exception(Ex.MOBJ.x100005);

            return default;
        }
        protected void AddHandlers<HandlerType1, HandlerType2>() where HandlerType1 : new() where HandlerType2 : new()
        {
            AddHandler<HandlerType1>(); AddHandler<HandlerType2>();
        }
        protected void AddHandlers<HandlerType1, HandlerType2, HandlerType3>() where HandlerType1 : new() where HandlerType2 : new() where HandlerType3 : new()
        {
            AddHandler<HandlerType1>(); AddHandler<HandlerType2>(); AddHandler<HandlerType3>();
        }
        protected HandlerType AddHandler<HandlerType>(string pKey) where HandlerType : new()
        {
            if (InstantiatePublicHandler(new HandlerType(), out HandlerType handlerResult, pKey))
            {
                return handlerResult;
            }
            else
                Exception(Ex.MOBJ.x100005);

            return default;
        }
        protected HandlerType AddHandler<HandlerType>(int pKey) where HandlerType : new() 
            => AddHandler<HandlerType>(pKey.ToString());

        protected HandlerType[] AddHandler<HandlerType>(string pKey1, string pKey2) where HandlerType : new()
        {
            HandlerType handler1 = AddHandler<HandlerType>(pKey1);
            HandlerType handler2 = AddHandler<HandlerType>(pKey2);

            return new HandlerType[2] { handler1, handler2 };
        }
        protected HandlerType[] AddHandler<HandlerType>(string pKey1, string pKey2, string pKey3) where HandlerType : new()
        {
            HandlerType handler1 = AddHandler<HandlerType>(pKey1);
            HandlerType handler2 = AddHandler<HandlerType>(pKey2);
            HandlerType handler3 = AddHandler<HandlerType>(pKey3);

            return new HandlerType[3] { handler1, handler2, handler3 };
        }
        protected HandlerType AddHandler<HandlerType>(Object pLocalBuffer) where HandlerType : new()
        {
            if (InstantiatePublicHandler(new HandlerType(), out HandlerType handlerResult))
            {
                if (handlerResult is ILocalBuffer handlerLocalBufferReduse)
                {
                    handlerLocalBufferReduse.SetBuffer(pLocalBuffer);
                }
                else
                    Exception(Ex.MOBJ.x100012);

                return handlerResult;
            }
            else
                Exception(Ex.MOBJ.x100005);

            return default;
        }
        protected void AddHandler<HandlerType1, HandlerType2>(Object pLocalBuffer) 
            where HandlerType1 : new() where HandlerType2 : new()
        {
            AddHandler<HandlerType1>(pLocalBuffer); AddHandler<HandlerType2>(pLocalBuffer);
        }
        protected void AddHandler<HandlerType1, HandlerType2>()
            where HandlerType1 : new() where HandlerType2 : new()
        {
            AddHandler<HandlerType1>(); AddHandler<HandlerType2>();
        }
        protected HandlerType AddHandler<HandlerType>(Object[] pLocalBuffers) where HandlerType : new()
        {
            if (InstantiatePublicHandler(new HandlerType(), out HandlerType handlerResult))
            {
                if (handlerResult is ILocalBuffers handlerLocalBufferReduse)
                {
                    handlerLocalBufferReduse.SetBuffers(pLocalBuffers);
                }
                else
                    Exception(Ex.MOBJ.x100012);

                return handlerResult;
            }
            else
                Exception(Ex.MOBJ.x100005);

            return default;
        }
        protected HandlerType AddHandler<HandlerType>(string pKey, Object pLocalBuffer) where HandlerType : new()
        {
            if (InstantiatePublicHandler(new HandlerType(), out HandlerType handlerResult, pKey))
            {
                if (handlerResult is ILocalBuffer handlerLocalBufferReduse)
                {
                    handlerLocalBufferReduse.SetBuffer(pLocalBuffer);
                }
                else
                    Exception(Ex.MOBJ.x100012);

                return handlerResult;
            }
            else
                Exception(Ex.MOBJ.x100005);

            return default;
        }
        protected HandlerType AddHandler<HandlerType>(string pKey, Object[] pLocalBuffers) where HandlerType : new()
        {
            if (InstantiatePublicHandler(new HandlerType(), out HandlerType handlerResult, pKey))
            {
                if (handlerResult is ILocalBuffers handlerLocalBufferReduse)
                {
                    handlerLocalBufferReduse.SetBuffers(pLocalBuffers);
                }
                else
                    Exception(Ex.MOBJ.x100013);

                return handlerResult;
            }
            else
                Exception(Ex.MOBJ.x100005);

            return default;
        }
        protected HandlerType GetHandler<HandlerType>() where HandlerType : new()
        {
            foreach (var handler in PublicHandlers)
            {
                if (handler.Value is HandlerType handlerReduse)
                {
                    return handlerReduse;
                }
            }   

            Exception(Ex.MOBJ.x100031);

            return default;
        }
        private bool TryGetHandler<HandlerType>(out HandlerType oHandler) where HandlerType : new()
        {
            foreach (var handler in PublicHandlers)
            {
                if (handler.Value is HandlerType handlerReduse)
                {
                    oHandler = handlerReduse;
                    return true;
                }
            }

            oHandler = default;
            return false;
        }
        private bool ContainsKeyHandler<HandlerType>() where HandlerType : new()
        {
            foreach (var handler in PublicHandlers)
            {
                if (handler.Value is HandlerType handlerReduse)
                {
                    return true;
                }
            }

            return false;
        }
        protected HandlerType GetHandler<HandlerType>(string pKey) where HandlerType : new()
        {
            foreach (var handler in PublicHandlers)
            {
                if (handler.Key == pKey)
                {
                    if (handler.Value is HandlerType handlerReduse)
                    {
                        return handlerReduse;
                    }
                }
            }

            Exception(Ex.MOBJ.x100030);

            return default;
        }

        protected HandlerType GetHandler<HandlerType>(int pKey) where HandlerType : new() 
            => GetHandler<HandlerType>(pKey.ToString());

        #endregion

        #region Hellpers

        private readonly List<IHellper> HellperList = new List<IHellper>();

        public HellperType AddHellper<HellperType>(HellperType pHellper)
        {
            if (pHellper is IHellper hellperReduse)
            {
                hellperReduse.SetHeader(this, Console, Exception);
                HellperList.Add(hellperReduse);
            }
            else
                Exception(Ex.MOBJ.x100035);

            return pHellper;
        }

        void IScopeHellper.AddObject<ObjectType>() => AddObject<ObjectType>();
        void IScopeHellper.AddObject<ObjectType>(string pKey, object pLocalBuffer) { AddObject<ObjectType>(pKey, pLocalBuffer); }
        void IScopeHellper.RemoveObject(string pKey) => RemoveObject(pKey);
        void IScopeHellper.SendToChildren<BufferType>(string pChildrenKey, BufferType pBuffer) { SendToChildren(pChildrenKey, pBuffer); }
        bool IScopeHellper.ContainsKeyObject(string pKey) => ContainsKeyObject(pKey);

        #endregion

        #region Dependency

        #region Stream

        private readonly List<Func<bool>> DependencysStream = new List<Func<bool>>();

        /// <summary>
        /// Добовлять зависимости только в конструкторе!!!!!
        /// </summary>
        /// <param name="pDependencyStream"></param>
        void IDependency.IStream.AddDependencyStream(System.Func<bool> pDependencyStream)
        {
            DependencysStream.Add(pDependencyStream);
        }

        #endregion

        #region Objects

        #region PrivateHandlers

        /*******************************************************************************
         * Приватные обработчики, входят в состав зависимостей. К ним невозможно получить 
         * доступ в нутри обьекта. Их создает PublicHandler в методе 
         * redirect_to<PrivateHandlerType>()
        *******************************************************************************/

        private readonly Dictionary<string, object> PrivateHandlers = new Dictionary<string, object>();
        private int PrivateHandlerKeyIndex = 0;

        private bool InstantiatePrivateHandler<HandlerType>(HandlerType pHandler, out HandlerType rReturnHandler)
        {
            rReturnHandler = default;

            bool result = true;
            {
                if (pHandler is MainObject controllerObjectReduse)
                {
                    if (controllerObjectReduse.GetTypeObject() == TypeObjectData.HANDLER)
                    {
                        string key = PrivateHandlerKeyIndex++.ToString();

                        if (PrivateHandlers.TryGetValue(key, out Object handlerObject))
                        {
                            result = Exception(Ex.MOBJ.x100008);
                        }
                        else
                        {
                            ((IDOM)controllerObjectReduse).SetDOM(Program, this, key, NodeNumber + 1);

                            PrivateHandlers.Add(key, pHandler);

                            if (controllerObjectReduse is HandlerType returnHandlerReduse)
                            {
                                rReturnHandler = returnHandlerReduse;
                            }
                            else result = false;
                        }
                    }
                    else result = Exception(Ex.MOBJ.x100007);
                }
                else result = Exception(Ex.MOBJ.x100006);
            }

            return result;
        }

        System.Func<PrivateHandlerType> IDependency.IObject.AddPrivateHandler<PrivateHandlerType>()
        {
            if (InstantiatePrivateHandler(new PrivateHandlerType(), out PrivateHandlerType privateHandlerReduse))
            {
                System.Func<PrivateHandlerType> getHandler = () => 
                {
                    return privateHandlerReduse;
                };

                return getHandler;
            }
            else
                Exception(Ex.MOBJ.x100032);

            return default;
        }

        System.Func<PrivateHandlerType> IDependency.IObject.AddPrivateHandler<PrivateHandlerType>(Object pLocalBuffer)
        {
            if (InstantiatePrivateHandler(new PrivateHandlerType(), out PrivateHandlerType privateHandlerReduse))
            {
                if (privateHandlerReduse is ILocalBuffer handlerLocalBufferReduse)
                {
                    handlerLocalBufferReduse.SetBuffer(pLocalBuffer);

                    System.Func<PrivateHandlerType> getHandler = () =>
                    {
                        return privateHandlerReduse;
                    };

                    return getHandler;
                }
                else
                    Exception(Ex.MOBJ.x100012);  
            }
            else
                Exception(Ex.MOBJ.x100032);

            return default;
        }
        System.Func<PrivateHandlerType> IDependency.IObject.AddPrivateHandler<PrivateHandlerType>(Object[] pLocalBuffers)
        {
            if (InstantiatePrivateHandler(new PrivateHandlerType(), out PrivateHandlerType privateHandlerReduse))
            {
                if (privateHandlerReduse is ILocalBuffers handlerLocalBufferReduse)
                {
                    handlerLocalBufferReduse.SetBuffers(pLocalBuffers);

                    System.Func<PrivateHandlerType> getHandler = () =>
                    {
                        return privateHandlerReduse;
                    };

                    return getHandler;
                }
                else
                    Exception(Ex.MOBJ.x100013);
            }
            else
                Exception(Ex.MOBJ.x100032);

            return default;
        }

        #endregion

        #endregion

        #endregion

        #region System

        private struct StateData
        {
            public const string CREATING = "Creating"; // В конструкторе.
            public const string START = "Start"; // Обьект запущен.
            public const string STARTING = "Starting"; // Обьект запускается.
            public const string PAUSE = "Pause"; // Обьект находится в состоянии ожидания.
            public const string PAUSING = "Pausing"; // Обьект готовится перейти в сотояние ожидания.
            public const string RESUME = "Resume"; // Обьект продолжает свою работу после паузы.
            public const string RESUMING = "Resuming"; // Обьект готовится продолжить свою работу после паузы.
            public const string STOP = "Stop"; // Обьект остановлен.
            public const string STOPPING = "Stopping"; // Обьект готовится к остановке.

            public const string EXCEPTION = "Exeption"; // В обьекте произошла ошибка, скора обькт остановится.
            public const string DESTROY = "Destroy"; // Обьект готов к его удалению.
        }

        private readonly SafeString State = new SafeString(StateData.CREATING);

        /// <summary>
        /// Работает ли обьет
        /// </summary>
        public bool StartProcess { get { return State.Value == StateData.START; } }
        /// <summary>
        /// Работает ли обьет
        /// </summary>
        public bool __StartProcess { get { return State.Comparison(StateData.START); } }
        /// <summary>
        /// Создание обьекта.
        /// </summary>
        public bool __IsCreating { get { return State.Comparison(StateData.CREATING); } }
        /// <summary>
        /// Обьект готовится. Запускаются потоки, зависимости и дт...
        /// </summary>
        public bool __IsStarting { get { return State.Comparison(StateData.STARTING); } }
        /// <summary>
        /// В нутри обьекта произошла ошибка.
        /// </summary>
        public bool __IsException { get { return State.Comparison(StateData.EXCEPTION); } }
        /// <summary>
        /// Обьект полностью остановил свою работу.
        /// </summary>
        public bool __IsDestroyed { get { return State.Comparison(StateData.STOP); } }
        /// <summary>
        /// Обьект готов для полного удаления.
        /// </summary>
        public bool __IsDestroy { get { return State.Comparison(StateData.DESTROY); } }

        public string GetState() { return State.Get(); }
        public string __GetState() { return State.__Get(); }

        void IStateObject.Construction() 
        {
            Construction();

            foreach (var pair in PrivateHandlers)
                ((IStateObject)pair.Value).Construction();

            foreach (var pair in PublicHandlers)
                ((IStateObject)pair.Value).Construction();

            foreach (var pair in ObjectsController)
                ((IStateObject)pair.Value).Construction();
        }
        void IStateObject.Dependency()
        {
            foreach (var dependency in DependencysStream)
            {
                if (dependency.Invoke())
                {
                    //...
                }
                else
                    Exception(Ex.MOBJ.x100001);
            }
        }

        void IStateObject.Start() { StartObj(); }
        void IStateObject.Stop() { StopObj(); }
        void IStateObject.Resume() { }
        void IStateObject.Pause() { }
        void IStateObject.Reboot() { }

        protected abstract void Construction();
        protected virtual void Configurate() { }
        protected virtual void Start() { }
        protected virtual void Stop() {  }
        protected virtual void Pause() { }
        protected virtual void Resume() { }

        protected void Destroy()
        {
            if (Flag(StateData.START, StateData.STARTING, StateData.CREATING, StateData.EXCEPTION))
            {
                ((IDOM)this).DestroyNode();
            }
        }

        protected virtual void StartObj()
        {
            if (Flag(StateData.CREATING, StateData.STARTING))
            {
                Configurate();

                if (__IsStarting)
                {
                    foreach (var pair in PublicHandlers)
                        ((IStateObject)pair.Value).Start();

                    foreach (var pair in PrivateHandlers)
                        ((IStateObject)pair.Value).Start();

                    foreach (var pair in ObjectsController)
                        ((IStateObject)pair.Value).Start();

                    Start();

                    Flag(StateData.STARTING, StateData.START);
                }
            }
        }

        protected virtual void StopObj()
        {
            if (Flag(StateData.CREATING, StateData.STARTING, StateData.START, StateData.PAUSE, 
                        StateData.EXCEPTION, StateData.STOPPING))
            {
                Stop();

                foreach (var pair in PublicHandlers)
                    ((IStateObject)pair.Value).Stop();

                foreach (var pair in PrivateHandlers)
                    ((IStateObject)pair.Value).Stop();

                foreach (var pair in ObjectsController)
                    ((IStateObject)pair.Value).Stop();

                foreach (var hellper in HellperList) hellper.Dispose();

                Flag(StateData.STOPPING, StateData.STOP);

                Dispose();
            }
        }

        protected void SystemInformation(string pMessage)
        {
            system.Console.WriteLine($"{GetExplorerObject()}:{pMessage}", ConsoleColor.Green);
        }

        private bool Flag(string pCurrentFlag, string pExposeFlag)
        {
            bool result = true;
            {
                if (State.Replace(pCurrentFlag, pExposeFlag))
                {
                    SystemInformation(State.Get());
                }
                else result = false;
            }
            return result;
        }
        private bool Flag(string pCurrentFlag1, string pCurrentFlag2, string pExposeFlag)
        {
            bool result = true;
            {
                if (State.Replace(pCurrentFlag1, pCurrentFlag2, pExposeFlag))
                {
                    SystemInformation(State.Get());
                }
                else result = false;
            }
            return result;
        }
        private bool Flag(string pCurrentFlag1, string pCurrentFlag2, string pCurrentFlag3, string pExposeFlag)
        {
            bool result = true;
            {
                if (State.Replace(pCurrentFlag1, pCurrentFlag2, pCurrentFlag3, pExposeFlag))
                {
                    SystemInformation(State.Get());
                }
                else result = false;
            }
            return result;
        }
        private bool Flag(string pCurrentFlag1, string pCurrentFlag2, string pCurrentFlag3, string pCurrentFlag4, string pExposeFlag)
        {
            bool result = true;
            {
                if (State.Replace(pCurrentFlag1, pCurrentFlag2, pCurrentFlag3, pCurrentFlag4, pExposeFlag))
                {
                    SystemInformation(State.Get());
                }
                else result = false;
            }
            return result;
        }
        private bool Flag(string pCurrentFlag1, string pCurrentFlag2, string pCurrentFlag3, string pCurrentFlag4, string pCurrentFlag5, string pExposeFlag)
        {
            bool result = true;
            {
                if (State.Replace(pCurrentFlag1, pCurrentFlag2, pCurrentFlag3, pCurrentFlag4, pCurrentFlag5, pExposeFlag))
                {
                    SystemInformation(State.Get());
                }
                else result = false;
            }
            return result;
        }

        #endregion

        #region Console, Exception

        /// <summary>
        /// Обьект прекрощает свою работу и уничтожается.
        /// </summary>
        protected bool Exception(string pMessage)
        {
            if (Flag(StateData.START, StateData.STARTING, StateData.CREATING, StateData.EXCEPTION))
            {
                string str = "\n███████╗██╗░░██╗░█████╗░███████╗██████╗░████████╗██╗░█████╗░███╗░░██╗\n" +
                               "██╔════╝╚██╗██╔╝██╔══██╗██╔════╝██╔══██╗╚══██╔══╝██║██╔══██╗████╗░██║\n" +
                               "█████╗░░░╚███╔╝░██║░░╚═╝█████╗░░██████╔╝░░░██║░░░██║██║░░██║██╔██╗██║\n" +
                               "██╔══╝░░░██╔██╗░██║░░██╗██╔══╝░░██╔═══╝░░░░██║░░░██║██║░░██║██║╚████║\n" +
                               "███████╗██╔╝╚██╗╚█████╔╝███████╗██║░░░░░░░░██║░░░██║╚█████╔╝██║░╚███║\n" +
                               "╚══════╝╚═╝░░╚═╝░╚════╝░╚══════╝╚═╝░░░░░░░░╚═╝░░░╚═╝░╚════╝░╚═╝░░╚══╝\n";

                string str1 = "\n********************************************************************\n";
                string str2 = "\n********************************************************************\n";

                system.Console.WriteLine($"{GetExplorerObject()}:{str + str1 + pMessage + str2}", System.ConsoleColor.Red);

                ((IDOM)this).DestroyNode();
            }

            return false;
        }

        protected void Exception(Exception pException)
        {
            Exception(pException.ToString());
        }
        protected void Exception(string pMessage, string pStr1)
        {
            if (new Regex(@"{\d}").Matches(pMessage).Count == 1)
                Exception(string.Format(pMessage, pStr1));
            else
                Exception(Ex.x1000000, pMessage);
        }
        protected void Exception(string pMessage, string pStr1, string pStr2)
        {
            if (new Regex(@"{\d}").Matches(pMessage).Count == 2)
                Exception(string.Format(pMessage, pStr1, pStr2));
            else
                Exception(Ex.x1000000, pMessage);
        }
        protected void Exception(string pMessage, string pStr1, string pStr2, string pStr3)
        {
            if (new Regex(@"{\d}").Matches(pMessage).Count == 3)
                Exception(string.Format(pMessage, pStr1, pStr2, pStr3));
            else
                Exception(Ex.x1000000, pMessage);
        }
        protected void Exception(string pMessage, string pStr1, string pStr2, string pStr3, string pStr4)
        {
            if (new Regex(@"{\d}").Matches(pMessage).Count == 4)
                Exception(string.Format(pMessage, pStr1, pStr2, pStr3, pStr4));
            else
                Exception(Ex.x1000000, pMessage);
        }

        /// <summary>
        /// Не безопасно выводит сообщение в консоль.
        /// </summary>
        protected void SystemConsoleWrite(string pMessage)
        {
            System.ConsoleColor consoleColor = System.ConsoleColor.White;

            if (__IsCreating)
            {
                consoleColor = System.ConsoleColor.Green;
            }

            System.Console.Write(pMessage, consoleColor);
        }

        /// <summary>
        /// Безопасно выводит сообщение в консоль.
        /// </summary>
        /// <param name="pMessage"></param>
        protected void Console(string pMessage)
        {
            System.ConsoleColor consoleColor = System.ConsoleColor.White;

            if (__IsCreating)
            {
                consoleColor = System.ConsoleColor.Green;
            }

            var p = "/"; int i = 0; string count = "";

            while ((i = Explorer.IndexOf(p, i)) != -1) { count += "  "; i += p.Length; }

            system.Console.WriteLine($"{GetExplorerObject()}:{pMessage}", ConsoleColor.Green);
        }

        /// <summary>
        /// Не безопасно выводит сообщение в консоль.
        /// </summary>
        /// <param name="pMessage"></param>
        protected void ConsoleWrite(string pMessage)
        {
            System.ConsoleColor consoleColor = System.ConsoleColor.White;

            if (__IsCreating)
            {
                consoleColor = System.ConsoleColor.Green;
            }

            system.Console.Write(pMessage, consoleColor);
        }

        #endregion

        #region Destroy

        public virtual void Dispose()
        {
            PublicHandlers.Clear();

            Flag(StateData.STOP, StateData.DESTROY);
        }

        #endregion
    }
}


