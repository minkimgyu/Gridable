using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SingleThreadPathfinding
{
    public interface INode<T> : IComparable<T>
    {
        int GetIndex();
        void ResetIndex(int index);
        public void Dispose();
    }

    public class Heap<T> where T : INode<T>
    {
        public Heap(int maxItemCount)
        {
            _items = new T[maxItemCount];
            _count = 0;
        }

        int getLastIndexHavingChild()
        {
            return (_count - 2) / 2;
        }

        int getParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        int getChild(int index, bool isLeft = true)
        {
            if (isLeft) return 2 * index + 1;
            else return 2 * index + 2;
        }

        void swap(int index1, int index2)
        {
            T tmp = _items[index2];
            _items[index2] = _items[index1];
            _items[index1] = tmp;

            _items[index1].ResetIndex(index1);
            _items[index2].ResetIndex(index2);
        }


        void percolateUp(int index)
        {
            int currentIndex = index;
            int parentIndex = getParentIndex(index);

            while (currentIndex > 0 && _items[parentIndex].CompareTo(_items[currentIndex]) > 0)
            {
                swap(parentIndex, currentIndex);

                currentIndex = parentIndex;
                parentIndex = getParentIndex(parentIndex);
            }
        }

        void percolateDown(int index)
        {
            int currentIndex = index;
            int childIndex = returnMinIndexInChildren(index);

            while (childIndex < _count && _items[childIndex].CompareTo(_items[currentIndex]) < 0)
            {
                swap(childIndex, currentIndex);

                if (childIndex > getLastIndexHavingChild()) break; // �ڽ��� �������� �ʴ� ��� Ż��

                currentIndex = childIndex;
                childIndex = returnMinIndexInChildren(childIndex); // ���� ���ֱ�
            }
        }

        int returnMinIndexInChildren(int index)
        {
            int leftChildIndex = getChild(index);
            int rightChildIndex = getChild(index, false);

            T leftChild = _items[leftChildIndex];
            T rightChild = _items[rightChildIndex];

            // right�� �� ���� ���
            if (rightChild != null && rightChild.CompareTo(leftChild) < 0) return rightChildIndex;
            return leftChildIndex;
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _items[i].Dispose();
            }
            Array.Clear(_items, 0, Count);
            _count = 0;
        }

        public bool Contain(T item)
        {
            if (item.GetIndex() == -1) return false; // �ʱ�ȭ�� �� �Ǿ����� ���
            return Equals(_items[item.GetIndex()], item);
        }

        public void Insert(T item)
        {
            item.ResetIndex(_count);
            _items[_count] = item;
            percolateUp(_count);
            _count++;
        }

        public void DeleteMin()
        {
            if (_count == 0) return;
            else if (_count == 1)
            {
                _items[0].ResetIndex(-1);
                _count--;
                return;
            }

            T lastItem = _items[_count - 1];

            lastItem.ResetIndex(0);
            _items[0].ResetIndex(-1);
            _count--;

            _items[0] = lastItem; // ù��° ���������� �ٲ���

            percolateDown(0);
        }

        public T ReturnMin() { return _items[0]; }

        T[] _items;
        int _count = 0;
        public int Count { get { return _count; } }
    }
}