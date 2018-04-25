using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using TextAdventure.Game.Displayables.Models;

namespace TextAdventure.Game.Game.States
{
    public class GameStates : IStaticStates
    {
        private List<IState> _stateInstances = new List<IState>(); 

        
        public T Fetch<T>() where T : IState
        {
            foreach (var stateInstance in _stateInstances)
            {
                if (stateInstance.GetType() == typeof (T))
                {
                    return (T)stateInstance;
                }
            }
            _stateInstances.Add((T)Activator.CreateInstance(typeof(T)));
            return (T)_stateInstances.Single(i => typeof(T) == i.GetType());
        }
        

        public class Active : IState
        {
            private CompositeDisposable _activeStateDisposables;

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {
                Console.WriteLine(displayables.GetDisplayable<GameStatesDisplayables.ActiveDis>().message);

                _activeStateDisposables = new CompositeDisposable();

                _activeStateDisposables.Add(Observable.Create<string>(observer =>
                {
                    var timer = new System.Timers.Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += (se, ev) => observer.OnNext(ev.SignalTime + "!!!");
                    timer.Start();
                    return timer;
                })
                //.Subscribe(Console.WriteLine));
                .Subscribe());
            }

            public void DisposeServicesOnExit()
            {
                _activeStateDisposables.Dispose();
            }


            public string FetchIdentifier()
            {
                return this.GetType().Name;
            }
        }

        public class Inactive : IState
        {

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {
                Console.WriteLine(displayables.GetDisplayable<GameStatesDisplayables.InactiveDis>().message);
            }

            public void DisposeServicesOnExit()
            {
            }

            public string FetchIdentifier()
            {
                return this.GetType().Name;
            }
        }

        public class Paused : IState
        {

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {
            }

            public void DisposeServicesOnExit()
            {
            }
            public string FetchIdentifier()
            {
                return this.GetType().Name;
            }
        }
        public class Running : IState
        {

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {
            }

            public void DisposeServicesOnExit()
            {
            }

            public string FetchIdentifier()
            {
                return this.GetType().Name;
            }
        }
        public class Exited : IState
        {

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {

            }

            public void DisposeServicesOnExit()
            {

            }

            public string FetchIdentifier()
            {
                return this.GetType().Name;
            }
        }
        public class Error : IState
        {

            public void ExecuteServicesOnEntry(bool isReentry, IDisplayables displayables)
            {

            }

            public void DisposeServicesOnExit()
            {

            }

            public string FetchIdentifier()
            {
                return this.GetType().Name;
            }
        }

    
    }

}
