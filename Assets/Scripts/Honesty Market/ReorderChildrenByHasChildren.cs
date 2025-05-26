using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Continuously reorders the direct children of this object at runtime so that:
/// - All children with no children are first,
/// - All children that have at least one child are last (at the bottom).
/// Attach to the parent (BigParent) object.
/// </summary>
public class ReorderChildrenByHasChildren : MonoBehaviour
{
	private int lastChildCount = -1;
	private readonly List<int> lastChildChildCounts = new List<int>();

	void Update()
	{
		if (ChildrenOrTheirCountsChanged())
		{
			Reorder();
		}
	}

	/// <summary>
	/// Returns true if the number of children, or any child's childCount, has changed.
	/// </summary>
	private bool ChildrenOrTheirCountsChanged()
	{
		int childCount = transform.childCount;
		if (childCount != lastChildCount)
		{
			CacheChildCounts();
			return true;
		}

		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			int cc = child.childCount;
			if (cc != lastChildChildCounts[i])
			{
				CacheChildCounts();
				return true;
			}
		}

		return false;
	}

	private void CacheChildCounts()
	{
		lastChildCount = transform.childCount;
		lastChildChildCounts.Clear();
		for (int i = 0; i < lastChildCount; i++)
		{
			lastChildChildCounts.Add(transform.GetChild(i).childCount);
		}
	}

	/// <summary>
	/// Reorder children: children with no children go to the top, those with children go to the bottom.
	/// </summary>
	public void Reorder()
	{
		int childCount = transform.childCount;
		if (childCount <= 1)
			return;

		List<Transform> leaves = new List<Transform>();
		List<Transform> branches = new List<Transform>();

		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.childCount == 0)
				leaves.Add(child);
			else
				branches.Add(child);
		}

		int idx = 0;
		foreach (var leaf in leaves)
			leaf.SetSiblingIndex(idx++);
		foreach (var branch in branches)
			branch.SetSiblingIndex(idx++);
	}
}
