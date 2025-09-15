using Breaddog.Extensions;
using System.Collections.Generic;
using System;
using Mirror;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class Entity : NetworkBehaviour
    {
        protected Controller controller;
        protected HashSet<Abillity> abillities;


        public Controller Controller => controller;
        public IReadOnlyCollection<Abillity> Abillities => abillities;

        public static Entity ObservingEntity { get; protected set; }


        private void Awake()
        {
            controller = GetComponent<Controller>();
            abillities = new(GetComponents<Abillity>());
        }

        public override void OnStartLocalPlayer()
        {
            ObservingEntity = this;
        }

        public override void OnStopLocalPlayer()
        {
            ObservingEntity = null;
        }

        // naming for easier debugging
        public override void OnStartClient()
        {
            name = $"Player[{netId}|{(isLocalPlayer ? "local" : "remote")}]";

            // Host Init() calls in OnStartServer
            if (isClientOnly)
                Init();
        }

        public override void OnStartServer()
        {
            name = $"Player[{netId}|server]";
            Init();
        }


        private void Init()
        {
            foreach (var abillity in Abillities)
            {
                try
                {
                    abillity.Init(this);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            controller.Init(this);
        }

        #region Set Controller

        /// <summary>
        /// Adds the controller and sets it to entity
        /// </summary>
        public virtual bool SetController<T>() where T : Controller
        {
            T addedController = gameObject.AddComponent<T>();
            return SetController(addedController);
        }

        /// <summary>
        /// Adds the controller and sets it to entity
        /// </summary>
        public virtual bool SetController(Type controller)
        {
            var addedController = gameObject.AddComponent(controller);
            return SetController(addedController as Controller);
        }

        /// <summary>
        /// Sets existing controller to entity
        /// </summary>
        public virtual bool SetController(Controller controller)
        {
            if (controller == null)
                return false;

            if (controller == this.controller)
                return false;

            // Remove current controller because cant be two controllers
            if (Controller != null)
                Destroy(Controller);

            this.controller = controller;

            return true;
        }

        #endregion

        #region Find Abillity

        public T FindAbillity<T>() where T : Abillity
        {
            return Abillities.FindByType<T>();
        }

        public Abillity FindAbillity(Type type)
        {
            return Abillities.FindByType(type) as Abillity;
        }



        public bool TryFindAbillity<T>(out T abillity) where T : Abillity
        {
            abillity = FindAbillity<T>();

            return abillity != null;
        }

        public bool TryFindAbillity(Type type, out Abillity abillity)
        {
            abillity = FindAbillity(type);

            return abillity != null;
        }

        #endregion
    }
}
