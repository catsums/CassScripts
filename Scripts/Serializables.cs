using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Serializables{

	#nullable enable
	[Serializable]
	public class SerialList<T> : List<T>{
		public T[] elements = new T[0];

		public new T this[int i]{
			get {
				return elements[i];
			}set { 
				elements[i] = value;
			}
		}

		public new int Capacity{
			get{
				return elements.Length;
			}set{
				if(value<0) return;
				T[] newElements = new T[value];

				int len = Math.Min(elements.Length, newElements.Length);

				for(int i=0;i<len;i++){
					newElements[i] = elements[i];
				}

				elements = newElements;
			}
		}

		public new int Count{
			get{
				return elements.Length;
			}
		}

		public SerialList(){}
		public SerialList(T[] elems){
			elements = new T[elems.Length];
			for(int i=0;i<elements.Length;i++){
				elements[i] = elems[i];
			}
		}
		public SerialList(IEnumerable<T> elems){
			T[] arr = elems.ToArray();
			elements = new T[arr.Length];
			for(int i=0;i<elements.Length;i++){
				elements[i] = arr[i];
			}
		}

		public new void Add(T item){
			int index = Count;
			elements = ResizeArray(elements, Count+1);
			elements[index] = item;
		}
		public new void AddRange(IEnumerable<T> items){
			T[] arr = items.ToArray();
			foreach(T item in arr){
				Add(item);
			}
		}
		public new System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly(){
			return new List<T>(elements).AsReadOnly();
		}
		public new int BinarySearch(T item){
			return new List<T>(elements).BinarySearch(item);
		}
		public new void Clear(){
			elements = new T[0];
		}
		public new bool Contains(T item){
			return elements.Contains(item);
		}
		public new List<TOutput> ConvertAll<TOutput> (Converter<T,TOutput> converter){
			return new List<T>(elements).ConvertAll(converter);
		}
		public new void CopyTo(T[] array, int arrayIndex){
			elements.CopyTo(array, arrayIndex);
		}public new void CopyTo(int index, T[] array, int arrayIndex, int count){
			new List<T>(elements).CopyTo(index,array,arrayIndex,count);
		}public new void CopyTo(T[] array){
			new List<T>(elements).CopyTo(array);
		}

		public new bool Exists(Predicate<T> match){
			return Array.Exists(elements,match);
		}

		public bool IsEmpty(){
			return (Count > 0);
		}

		#nullable enable
		public new T? Find(Predicate<T> match){
			return new List<T>(elements).Find(match);
		}
		public new List<T> FindAll(Predicate<T> match){
			return new List<T>(elements).FindAll(match);
		}
		public new int FindIndex(int startIndex, int count, Predicate<T> match){
			return Array.FindIndex(elements, startIndex, count, match);
		}public new int FindIndex(Predicate<T> match){
			return Array.FindIndex(elements, match);
		}public new int FindIndex(int startIndex, Predicate<T> match){
			return Array.FindIndex(elements, startIndex, match);
		}
		public new T? FindLast (Predicate<T> match){
			return Array.FindLast(elements, match);
		}
		public new int FindLastIndex(int startIndex, int count, Predicate<T> match){
			return Array.FindLastIndex(elements, startIndex, count, match);
		}public new int FindLastIndex(Predicate<T> match){
			return Array.FindLastIndex(elements, match);
		}public new int FindLastIndex(int startIndex, Predicate<T> match){
			return Array.FindLastIndex(elements, startIndex, match);
		}

		public new void ForEach(Action<T> action){
			Array.ForEach(elements, action);
		}

		public new Enumerator GetEnumerator(){
			return new List<T>(elements).GetEnumerator();
		}

		public new List<T> GetRange(int index, int count){
			return new List<T>(elements).GetRange(index,count);
		}

		public new int IndexOf(T item, int index){
			return Array.IndexOf(elements, item, index);
		}public new int IndexOf(T item, int index, int count){
			return Array.IndexOf(elements, item, index, count);
		}public new int IndexOf(T item){
			return Array.IndexOf(elements, item);
		}

		public new void Insert(int index, T item){
			T[] newArr = new T[elements.Length+1];
			for(int i=0;i<index;i++){
				newArr[i] = elements[i];
			}
			newArr[index] = item;
			for(int i=index+1;i<newArr.Length;i++){
				newArr[i] = elements[i-1];
			}

			elements = newArr;
		}

		public new void InsertRange(int index, IEnumerable<T> collection){
			T[] coll = collection.ToArray();

			T[] newArr = new T[elements.Length+1];
			for(int i=0;i<index;i++){
				newArr[i] = elements[i];
			}
			for(int i=0;i<coll.Length;i++){
				newArr[i + index] = coll[i]; 
			}
			for(int i=index+coll.Length;i<newArr.Length;i++){
				newArr[i] = elements[i-1];
			}

			elements = newArr;
		}

		public new int LastIndexOf(T item, int index){
			return Array.LastIndexOf(elements, item, index);
		}public new int LastIndexOf(T item, int index, int count){
			return Array.LastIndexOf(elements, item, index, count);
		}public new int LastIndexOf(T item){
			return Array.LastIndexOf(elements, item);
		}

		public new bool Remove(T item){
			List<T> list = new List<T>(elements);
			var x = list.Remove(item);
			elements = list.ToArray();

			return x;
		}
		public new int RemoveAll(Predicate<T> match){
			List<T> list = new List<T>(elements);
			var x = list.RemoveAll(match);
			elements = list.ToArray();

			return x;
		}
		public new void RemoveAt(int index){
			List<T> list = new List<T>(elements);
			list.RemoveAt(index);
			elements = list.ToArray();
		}
		public new void RemoveRange(int index, int count){
			List<T> list = new List<T>(elements);
			list.RemoveRange(index,count);
			elements = list.ToArray();
		}
		public new void Reverse(){
			Array.Reverse(elements);
		}
		public new void Reverse(int index, int count){
			Array.Reverse(elements,index,count);
		}
		public new void Sort(Comparison<T> comparison){
			Array.Sort(elements,comparison);
		}public new void Sort(){
			Array.Sort(elements);
		}public new void Sort(IComparer<T> comparer){
			Array.Sort(elements,comparer);
		}public new void Sort(int index, int count, IComparer<T>? comparer){
			Array.Sort(elements,index,count,comparer);
		}

		public new T[] ToArray(){
			return CopyArray(elements);
		}
		public new void TrimExcess(){
			List<T> list = new List<T>(elements);
			list.TrimExcess();
			elements = list.ToArray();
		}
		public new bool TrueForAll(Predicate<T> match){
			return Array.TrueForAll(elements,match);
		}

		//Stack/Queue
		#nullable disable
		public bool TryPeekFirst(out T item){
			try{
				item = PeekFirst();
				return true;
			}catch(Exception e){
				e.ToString();
				item = default(T);
			}
			return false;
		}

		public bool TryPeekLast(out T item){
			try{
				item = PeekLast();
				return true;
			}catch(Exception e){
				e.ToString();
				item = default(T);
			}
			return false;
		}
		public bool TryPeek(out T item){
			try{
				item = PeekLast();
				return true;
			}catch(Exception e){
				e.ToString();
				item = default(T);
			}
			return false;
		}
		#nullable enable
		public T? Peek(){
			return elements[Count];
		}
		public T? PeekLast(){
			return elements[Count];
		}
		public T? PeekFirst(){
			return elements[0];
		}
		#nullable disable

		public void Unshift(T item){
			Insert(0, item);
		}
		public void Push(T item){
			Insert(Count, item);
		}
		#nullable enable
		public T? Shift(){
			T item = elements[0];
			RemoveAt(0);
			return item;
		}
		public T? Pop(){
			T item = elements[Count-1];
			RemoveAt(Count-1);
			return item;
		}
		#nullable disable
		public bool TryShift(out T item){
			try{
				item = Shift();
				return true;
			}catch(Exception e){
				e.ToString();
				item = default(T);
			}
			return false;
		}
		public bool TryPop(out T item){
			try{
				item = Pop();
				return true;
			}catch(Exception e){
				e.ToString();
				item = default(T);
			}
			return false;
		}

		public object Clone(){
			return new SerialList<T>(CopyArray(elements));
		}

		T[] ResizeArray(T[] arr, int length){
			if(length<0) return new T[0];
			T[] newArr = new T[length];

			int len = Math.Min(arr.Length, newArr.Length);

			for(int i=0;i<len;i++){
				newArr[i] = arr[i];
			}

			return newArr;
		}
		T[] CopyArray(T[] arr){
			return ResizeArray(arr, arr.Length);
		}
	}
	
}