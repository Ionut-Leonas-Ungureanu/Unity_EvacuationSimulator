#pragma once
#include <iostream>
#include <algorithm>
#include "BufferNode.h"

template<typename T>
class Buffer
{
private:
	BufferNode<T>* front = NULL;
	BufferNode<T>* tail = NULL;
	int maxSize;

public:
	int size = 0;

	Buffer(int size)
	{
		if (size == 0)
		{
			throw std::invalid_argument("Queue size must be greater than 0.");
		}

		this->maxSize = size;
	}

	~Buffer()
	{
		if (IsEmpty())
		{
			return;
		}

		do
		{
			auto temp = front->next;
			delete front;
			front = temp;
		} while (front->next != NULL);

		delete front;
		front = NULL;
		tail = NULL;
	}

	bool IsEmpty()
	{
		if (front == NULL)
		{
			return true;
		}

		return false;
	}

	void Put(T item)
	{
		if (size == maxSize)
		{
			Pop();
		}

		if (IsEmpty())
		{
			// There is no element
			front = new BufferNode<T>();
			front->data = item;
			tail = front;
		}
		else
		{
			// There is one or more elements
			auto temp = new BufferNode<T>();
			temp->data = item;
			temp->next = front;
			front->previous = temp;
			front = temp;
		}

		++size;
	}

	T Pop()
	{
		if (IsEmpty())
		{
			throw std::out_of_range("Queue is empty!");
		}

		T result = tail->data;
		auto temp = tail;
		if (tail == front)
		{
			tail = front = NULL;
		}
		else
		{
			tail = tail->previous;
		}
		delete temp;

		--size;

		return result;
	}

	T* GetItemsChronologically(int indexes[], int indexesSize)
	{
		if (IsEmpty())
		{
			return NULL;
		}

		std::sort(indexes, indexes + indexesSize);

		auto k = 0;
		auto idx = 0;
		T* results = new T[indexesSize];
		BufferNode<T>* parser = tail;
		do
		{
			if (indexes[k] == idx)
			{
				results[k] = parser->data;
				++k;
				if (k == indexesSize)
				{
					return results;
				}
			}
			parser = parser->previous;
			++idx;
		} while (parser != NULL);

		delete[] results;
		return NULL;
	}
};