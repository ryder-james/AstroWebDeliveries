using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteList", menuName = "Utilities/Sprite List")]
public class SpriteList : ScriptableObject, IList<Sprite> {
	[SerializeField]
	private List<Sprite> sprites;

	public Sprite this[int index] { get => sprites[index]; set => sprites[index] = value; }

	public int Count => sprites.Count;

	public bool IsReadOnly => true;

	public void Add(Sprite item) => throw new System.AccessViolationException();
	public void Clear() => throw new System.AccessViolationException();
	public bool Contains(Sprite item) => sprites.Contains(item);
	public void CopyTo(Sprite[] array, int arrayIndex) => sprites.CopyTo(array, arrayIndex);
	public IEnumerator<Sprite> GetEnumerator() => sprites.GetEnumerator();
	public int IndexOf(Sprite item) => sprites.IndexOf(item);
	public void Insert(int index, Sprite item) => throw new System.AccessViolationException();
	public bool Remove(Sprite item) => throw new System.AccessViolationException();
	public void RemoveAt(int index) => throw new System.AccessViolationException();
	IEnumerator IEnumerable.GetEnumerator() => sprites.GetEnumerator();
}
