using System;
using System.Collections.Generic;
using SignalBusNS;
using Serializables;

namespace StateMachineNS{
	/// <summary>
	/// A state for the StateMachine so it can be read and used in a Graph data structure.
	/// While this can be visualised in a graph, its better to sort the nodes as a list of them when they are too many.
	/// The details for a state are not important since they only have the connections to other states and their name
	/// </summary>
	[Serializable]
	public class State{
		public string name;
		public List<State> connectedStates = new List<State>();

		public State(string n){
			name = n;
		}

		public bool ConnectState(State st){
			if(st==null) return false;
			if(!IsConnectedTo(st)){
				connectedStates.Add(st);

				return true;
			}
			return false;
		}
		public bool IsConnectedTo(State st){
			if(st==null) return false;
			if(connectedStates.Contains(st)) return true;
			return false;
		}
		public bool IsConnectedFrom(State st){
			if(st==null) return false;
			return st.IsConnectedTo(this);
		}
		public bool DisconnectState(State st){
			return connectedStates.Remove(st);
		}
	}
	/// <summary>
		/// The graph of the States that manages transitions.
		/// This connects and transitions through states like a directional graph.
		/// It allows for emitting signals to check for state changes so that the object with the machine can react in order.
	/// </summary>
	[Serializable]
	public class StateMachine{
		public SignalBus signalBus = new SignalBus();

		public static class Signals{
			public const string 
			StateCreate = "stateCreate",
			StateDelete = "stateDelete",
			StateConnect = "stateConnect",
			StateDisconnect = "stateDisconnect",
			StateChange = "stateChange";
		}
		protected Dictionary<string, State> states = new Dictionary<string, State>();
		
		protected SerialList<string> stateStack = new SerialList<string>();
		public string _current;
		public string _prev;
		private string _top{
			get{
				return GetNextStateName();
			}
		}

		/// <summary>
		/// Getter for the current state.
		/// </summary>

		public State CurrentState{
			get{
				if(HasState(_current)){
					return states[_current];
				}
				return null;
			}
		}
		/// <summary>
		/// Getter for the previous state.
		/// </summary>
		public State PreviousState{
			get{
				if(HasState(_prev)){
					return states[_prev];
				}
				return null;
			}
		}

		/// <summary>
		/// StateMachine Constructor
		/// </summary>

		public StateMachine(string curr = null){
			if(curr!=null){
				_current = curr;
				states[_current] = new State(curr);
			}
		}

		public StateMachine(State st = null){
			if(st!=null){
				_current = st.name;
				states[_current] = st;
			}
		}

		/// <summary>
		/// Creates a new state when inside an object to initialise it.
		/// </summary>

		public bool CreateState(string stateName){
			if(stateName==null) return false;
			if(HasState(stateName)) return false;

			states[stateName] = new State(stateName);
			
			signalBus.Emit(Signals.StateCreate, states[stateName]);
			return true;
		}
		public bool CreateStates(string[] stateNames){
			if(stateNames==null || stateNames.Length<=0) return false;

			bool succ = true;
			foreach(string st in stateNames){
				bool x = CreateState(st);
				if(!x) succ = false;
			}
			return succ;
		}
		/// <summary>
		/// Removes state from the statemachine.
		/// </summary>
		public State DeleteState(string stateName){
			if(!HasState(stateName)) return null;

			State stateToDelete = states[stateName];

			states.Remove(stateName);
			
			signalBus.Emit(Signals.StateDelete, stateToDelete);
			return stateToDelete;
		}

		/// <summary>
		/// Checks if state exists in the machine by name
		/// </summary>

		public bool HasState(string stateName){
			if(stateName==null) return false;
			if(states.ContainsKey(stateName)) return true;
			return false;
		}
		/// <summary>
		/// Connects two states together so they can transition from each other (in direction from to)
		/// </summary>
		public bool ConnectState(string fromState, string toState){
			if(!HasState(fromState) || !HasState(toState)) return false;

			var stFrom = states[fromState];
			var stTo = states[toState];
			if(stFrom == null || stTo == null) return false;
			
			if(stFrom.ConnectState(stTo)){
				signalBus.Emit(Signals.StateConnect, new Dictionary<string,object>(){
					{"from",stFrom}, {"to",stFrom},
				});
				return true;
			}
			return false;
		}
		/// <summary>
		/// Connects one state to multiple other states
		/// </summary>
		public bool ConnectStates(string fromState, string[] toStates){
			if(fromState==null) return false;
			if(toStates==null || toStates.Length<=0) return false;

			bool succ = true;
			foreach(string st in toStates){
				bool x = ConnectState(fromState, st);
				if(!x) succ = false;
			}
			return succ;
		}
		/// <summary>
		/// Connects multiple states to one state
		/// </summary>
		public bool ConnectStates(string[] fromStates, string toState){
			if(fromStates==null || fromStates.Length<=0) return false;
			if(toState==null) return false;

			bool succ = true;
			foreach(string st in fromStates){
				bool x = ConnectState(st, toState);
				if(!x) succ = false;
			}
			return succ;
		}
		/// <summary>
		/// Connects multiple states to multiple other states
		/// </summary>
		public bool ConnectStates(string[] fromStates, string[] toStates){
			if(fromStates==null || fromStates.Length<=0) return false;
			if(toStates==null || toStates.Length<=0) return false;

			bool succ = true;
			foreach(string _from in fromStates){
				foreach(string _to in fromStates){
					bool x = ConnectState(_from, _to);
					if(!x) succ = false;
				}
			}
			return succ;
		}
		/// <summary>
		/// Connects all specified states to each other in both directions
		/// </summary>
		public bool InterConnectStates(string[] _states){
			bool succ = true;
			foreach(string stA in _states){
				bool x = ConnectStates(stA, _states);
				if(!x) succ = false;
			}
			return succ;
		}
		/// <summary>
		/// Removes connection from one state to another
		/// </summary>
		public bool DisconnectState(string fromState, string toState){
			if(!HasState(fromState) || !HasState(toState)) return false;
			var stFrom = states[fromState];
			var stTo = states[toState];
			if(stFrom == null || stTo == null) return false;
			
			if(stFrom.DisconnectState(stTo)){
				signalBus.Emit(Signals.StateDisconnect, new Dictionary<string,object>(){
					{"from",stFrom}, {"to",stFrom},
				});
				return true;
			}
			return false;
		}
		/// <summary>
		/// Removes connection from one state to multiple other states
		/// </summary>
		public bool DisconnectStates(string fromState, string[] toStates){
			bool succ = true;
			foreach(string st in toStates){
				bool x = DisconnectState(fromState, st);
				if(!x) succ = false;
			}
			return succ;
		}
		/// <summary>
		/// Removes both connections multiple states
		/// </summary>
		public bool InterDisconnectStates(string[] _states){
			bool succ = true;
			foreach(string stA in _states){
				bool x = DisconnectStates(stA, _states);
				if(!x) succ = false;
			}
			return succ;
		}
		/// <summary>
		/// Switches from the current state to the new state, if it has a connection
		/// </summary>
		public bool SwitchToState(string stateName){
			if(!HasState(stateName)) return false;

			var newState = states[stateName];

			var oldState = CurrentState;
			if(oldState == null || oldState.IsConnectedTo(newState)){
				_current = newState.name;
				_prev = (oldState!=null ? oldState.name : null);

				signalBus.Emit(Signals.StateChange, new Dictionary<string, object>(){
					{"old",oldState}, {"new",newState},
				});
				return true;
			}
			return false;
		}
		/// <summary>
		/// Switches to the last state it was on
		/// </summary>
		public bool SwitchToPreviousState(){
			return SwitchToState(_prev);
		}

		/// <summary>
		/// Checks if machine can switch to state from current state
		/// </summary>
		public bool CanSwitchTo(string stateName){
			if(!HasState(stateName) || CurrentState==null) return false;
			State st = states[stateName];

			return CurrentState.IsConnectedTo(st);
		}
		/// <summary>
		/// Switches from current state to next state in the next state stack. This will pop from the state always for each attempt to switch to that state
		/// </summary>

		public bool SwitchToNextStateInStack(){
			string nextState;
			stateStack.TryPop(out nextState);

			if(nextState!=null) return SwitchToState(nextState);
			return false;
		}
		/// <summary>
		/// Pushes a state into the next state stack
		/// </summary>
		public void PushToStack(string stateName){
			if(HasState(stateName)){
				stateStack.Push(stateName);
			}
		}

		public string GetCurrentStateName(){
			if(CurrentState!=null) return CurrentState.name;
			return null;
		}
		public string GetPreviousStateName(){
			if(PreviousState!=null) return PreviousState.name;
			return null;
		}
		public string GetNextStateName(){
			string n = null;
			stateStack.TryPeek(out n);
			return n;
		}
		public bool isCurrentState(string n){
			return (_current == n);
		}
		public bool isPreviousState(string n){
			return (_prev == n);
		}
		public bool isNextState(string n){
			return (GetNextStateName() == n);
		}

	}
}