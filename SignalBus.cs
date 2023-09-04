using System;
using System.Linq;
using System.Collections.Generic;

namespace SignalBusLegacyNS{
	/// <summary>
	/// SignalBus allows for event handling between objects that have signal busses in order to subscribe to each other and emit signals to run functions.
	/// This is similar to C# Actions and Funcs, except this lets you keep track of handlers and connect to any Action taking in an object.
	/// This applies the Command Pattern and can potentially apply the Mediator or Observer pattern as well.
	/// <br/>                  
	/// Note: For handlers, please remember to type case the object to your desired type.
	/// This is very similar to javascript, but does not use coroutines - all actions are executed upon Emit().
	/// </summary>
		
    public class SignalBus{
        protected Dictionary< string, List< Action<object> > > handlers;
        protected Dictionary< string, Action<object> > signals;

		protected static SignalBus _global = null;

		public static SignalBus Global{
			get{
				if(_global == null){
					_global = new SignalBus();
				}
				return _global;
			}
		}

		public static void Broadcast(string n){
			Global.Emit(n);
		}
		public static void Broadcast(string n, object x){
			Global.Emit(n,x);
		}

		/*
			Constructor for signalbus
		*/
        public SignalBus(){
            handlers = new Dictionary< string, List< Action<object> > >();
            signals = new Dictionary< string, Action<object> >();
        }
		/*
			Creates a new signal with a certain name
		*/

        public virtual void CreateSignal(string n){
            handlers[n] = new List<Action<object>>();
            signals[n] = null;
        }
		/*
			Subscribes an action (function) to a signal name
		*/

        public virtual void Subscribe(string n, Action<object> listener){
            if(!HasSignal(n)){
                CreateSignal(n);
            }

            handlers[n].Add(listener);
            signals[n] += listener;

        }
		/*
			Unsubs an action from a signal name
		*/
        public virtual void Unsubscribe(string n, Action<object> listener){
            if(HasSignal(n)){
                signals[n] -= listener;
                handlers[n].Remove(listener);
            }
        }
		/*
			Checks if a specific function is subbed to a certain signal
		*/
        public virtual bool IsSubscribed(string n, Action<object> listener){
            if(HasSignal(n)){
                return handlers[n].Contains(listener);
            }
            return false;
        }
		/*
			Checks if a signal exists in this signalBus
		*/
        public virtual bool HasSignal(string n){
            if(signals.ContainsKey(n))
                return true;
            return false;
        }
		/*
			Emit signal of that name to invoke all actions/functions connected to it
		*/
        public virtual void Emit(string n){
            if(HasSignal(n)){
                signals[n]?.Invoke(null);
            }
        }
		/*
			Emit signal of that name to invoke all actions/functions connected to it with a parameter
		*/
        public virtual void Emit(string n, object data){
            if(HasSignal(n)){
                signals[n]?.Invoke(data);
            }
        }
    }
}

namespace SignalBusNS
{
	public class SignalEventData{
		protected Dictionary<string, object> data = new Dictionary<string, object>();

		public object this[string k]{
			get{
				return Get(k);
			}set{
				Set(k, value);
			}
		}
		public object this[string[] keys]{
			set{
				Set(keys, value);
			}
		}

		public SignalEventData(){
		}public SignalEventData(Dictionary<string,object> _data){
			var keys = _data.Keys.ToArray();
			foreach(var k in keys){
				data[k] = _data[k];
			}
		}

		public object Get(string k){
			if(data.ContainsKey(k)){
				return data[k];
			}
			return null;
		}
		public T Get<T>(string k){
			if(data.ContainsKey(k)){
				return (T) data[k];
			}
			return default(T);
		}

		public SignalEventData Set(string k, object x){
			data[k] = x;
			return this;
		}
		public SignalEventData Set<T>(string k, T x){
			data[k] = x;
			return this;
		}
		public SignalEventData Set(string[] keys, object x){
			foreach(var k in keys){
				Set(k, x);
			}
			return this;
		}
		public SignalEventData Set<T>(string[] keys, T x){
			foreach(var k in keys){
				Set(k, x);
			}
			return this;
		}
		
	}
	/// <summary>
	/// SignalBus allows for event handling between objects that have signal busses in order to subscribe to each other and emit signals to run functions.
	/// This is similar to C# Actions and Funcs, except this lets you keep track of handlers and connect to any Action taking in an object.
	/// This applies the Command Pattern and can potentially apply the Mediator or Observer pattern as well.
	/// <br/>
	/// This is very similar to javascript, but does not use coroutines - all actions are executed upon Emit().
	/// </summary>
    public class SignalBus{
        protected Dictionary< string, List< object > > handlers = new Dictionary<string, List<object>>();

		protected static SignalBus _global = null;

		/// <summary>
		/// Get the Global SignalBus used for Broadcasts
		/// </summary>
		public static SignalBus Global{
			get{
				if(_global == null){
					_global = new SignalBus();
				}
				return _global;
			}
		}

		/// <summary>
		/// Emit signal to all handlers connected to Global SignalBus
		/// </summary>
		public static void Broadcast(string n){
			Global.Emit(n);
		}
		public static void Broadcast(string n, object x){
			Global.Emit(n,x);
		}
		public static void Broadcast<T>(string n, T x){
			Global.Emit(n,x);
		}

		/// <summary>
		/// Constructs a new signal bus for functions to subscribe to
		/// </summary>
        public SignalBus(){

        }
		/// <summary>
		/// Constructs a new signal bus with preset signals created
		/// </summary>
		/// <param name="arr">Array of signal names</param>
        public SignalBus(string[] arr){
			foreach(var n in arr){
				CreateSignal(n);
			}
        }
		/// <summary>
		/// Creates a new signal with a certain name
		/// </summary>
		/// <param name="n">Name signal to create</param>

        public virtual void CreateSignal(string n){
            handlers[n] = new List< object >();
        }
		/// <summary>
		/// Subscribes an action (function) to a signal name
		/// </summary>
		/// <param name="n">Name of signal to subscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		/// <returns>The original SignalBus, so you can chain this function</returns>

        public virtual SignalBus Subscribe(string n, Action listener){
            if(!HasSignal(n)){
                CreateSignal(n);
            }

            handlers[n].Add(listener);
			return this;
        }public virtual SignalBus Subscribe(string n, Action<object> listener){
            if(!HasSignal(n)){
                CreateSignal(n);
            }

            handlers[n].Add(listener);
			return this;
        }public virtual SignalBus Subscribe<T>(string n, Action<T> listener){
            if(!HasSignal(n)){
                CreateSignal(n);
            }

            handlers[n].Add(listener);
			return this;
        }
		/// <summary>
		/// Subscribes an action (function) to multiple signals
		/// </summary>
		/// <param name="arr">Array of signals to subscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		
		/// <returns>The original SignalBus, so you can chain this function</returns>
		public virtual SignalBus Subscribe(string[] arr, Action listener){
			foreach(var n in arr){
				Subscribe(n,listener);
			}
			return this;
        }public virtual SignalBus Subscribe(string[] arr, Action<object> listener){
            foreach(var n in arr){
				Subscribe(n,listener);
			}
			return this;
        }public virtual SignalBus Subscribe<T>(string[] arr, Action<T> listener){
            foreach(var n in arr){
				Subscribe(n,listener);
			}
			return this;
        }
		/// <summary>
		/// Subscribes an action (function) to a signal name. Alias for Subscribe()
		/// </summary>
		///<param name="n">Name of signal to subscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		
		/// <returns>The original SignalBus, so you can chain this function</returns>
        public virtual SignalBus On(string n, Action listener){
        	return Subscribe(n,listener);
        }public virtual SignalBus On(string n, Action<object> listener){
            return Subscribe(n,listener);
        }public virtual SignalBus On<T>(string n, Action<T> listener){
        	return Subscribe(n,listener);
        }
		/// <summary>
		/// Subscribes an action (function) to multiple signals. Alias for Subscribe()
		/// </summary>
		/// <param name="n">Array of signals to subscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		
		/// <returns>The original SignalBus, so you can chain this function</returns>
        public virtual SignalBus On(string[] arr, Action listener){
        	return Subscribe(arr,listener);
        }public virtual SignalBus On(string[] arr, Action<object> listener){
            return Subscribe(arr,listener);
        }public virtual SignalBus On<T>(string[] arr, Action<T> listener){
        	return Subscribe(arr,listener);
        }
		/// <summary>
		/// Unsubscribes an action (function) from a signal name
		/// </summary>
		/// <param name="n">Name of signal to unsubscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		
		/// <returns>The original SignalBus, so you can chain this function</returns>
        public virtual SignalBus Unsubscribe(string n, Action listener){
            if(HasSignal(n)){
                handlers[n].Remove(listener);
            }
			return this;
        }public virtual SignalBus Unsubscribe(string n, Action<object> listener){
            if(HasSignal(n)){
                handlers[n].Remove(listener);
            }
			return this;
        }public virtual SignalBus Unsubscribe<T>(string n, Action<T> listener){
            if(HasSignal(n)){
                handlers[n].Remove(listener);
            }
			return this;
        }
		
        /// <summary>
		/// Unsubscribes an action (function) from many signals
		/// </summary>
		/// <param name="n">Array of signals to unsubscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		
		/// <returns>The original SignalBus, so you can chain this function</returns>
        public virtual SignalBus Unsubscribe(string[] arr, Action listener){
        	foreach(var n in arr){
				Unsubscribe(n,listener);
			}
			return this;
        }public virtual SignalBus Unsubscribe(string[] arr, Action<object> listener){
            foreach(var n in arr){
				Unsubscribe(n,listener);
			}
			return this;
        }public virtual SignalBus Unsubscribe<T>(string[] arr, Action<T> listener){
            foreach(var n in arr){
				Unsubscribe(n,listener);
			}
			return this;
        }
		/// <summary>
		/// Unsubscribes an action (function) from a signal name. Alias of Unsubscribe().
		/// </summary>
		/// <param name="n">Name of signal to unsubscribe to</param>
		/// <param name="listener">Function to run when signal is emitted</param>
		
		/// <returns>The original SignalBus, so you can chain this function</returns>
        public virtual SignalBus Off(string n, Action listener){
            return Unsubscribe(n,listener);
        }public virtual SignalBus Off(string n, Action<object> listener){
            return Unsubscribe(n,listener);
        }public virtual SignalBus Off<T>(string n, Action<T> listener){
            return Unsubscribe(n,listener);
        }
        public virtual SignalBus Off(string[] arr, Action listener){
            return Unsubscribe(arr,listener);
        }public virtual SignalBus Off(string[] arr, Action<object> listener){
            return Unsubscribe(arr,listener);
        }public virtual SignalBus Off<T>(string[] arr, Action<T> listener){
            return Unsubscribe(arr,listener);
        }
		/*
		*/
		/// <summary>
		/// Checks if a specific function is subbed to a certain signal
		/// </summary>
		/// <returns></returns>
        public virtual bool IsSubscribed(string n, Action listener){
            if(HasSignal(n)){
                return handlers[n].Contains(listener);
            }
            return false;
        }public virtual bool IsSubscribed(string n, Action<object> listener){
            if(HasSignal(n)){
                return handlers[n].Contains(listener);
            }
            return false;
        }public virtual bool IsSubscribed<T>(string n, Action<T> listener){
            if(HasSignal(n)){
                return handlers[n].Contains(listener);
            }
            return false;
        }
		/// <summary>
		/// Checks if a signal exists in this signalBus
		/// </summary>
		/// <returns></returns>
        public virtual bool HasSignal(string n){
            if(handlers.ContainsKey(n))
                return true;
            return false;
        }
		/// <summary>
		/// Emit signal of that name to invoke all actions/functions connected to it
		/// </summary>
        public virtual void Emit(string n){
            if(HasSignal(n)){
				var listeners = handlers[n].ToArray();
				foreach(var listener in listeners){
					var x = listener as Action;
					if(x != null){
						x();
						continue;
					}
					var y = listener as Action<object>;
					if(y != null){
						y(null);
						continue;
					}
				}
            }
        }
		/// <summary>
		/// Emit signal of that name to invoke all actions/functions connected to it with a parameter
		/// </summary>
        public virtual void Emit(string n, object data){
            if(HasSignal(n)){
				var listeners = handlers[n].ToArray();
				foreach(var listener in listeners){
					var x = listener as Action<object>;
					if(x != null){
						x(data);
						continue;
					}
					var y = listener as Action;
					if(y != null){
						y();
						continue;
					}
				}
            }
        }
		/// <summary>
		/// Emit signal of that name to invoke all actions/functions connected to it with a parameter of specific type
		/// </summary>
        public virtual void Emit<T>(string n, T data){
            if(HasSignal(n)){
				var listeners = handlers[n].ToArray();
				foreach(var listener in listeners){
					var x = listener as Action<T>;
					if(x != null){
						x(data);
						continue;
					}
					var y = listener as Action<object>;
					if(y != null){
						y(data);
						continue;
					}
					var z = listener as Action;
					if(z != null){
						z();
						continue;
					}
				}
            }
        }
    }
}