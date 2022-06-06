#pragma once

template <typename T>
struct BufferNode
{
public:
	T data;
	BufferNode* next = NULL;
	BufferNode* previous = NULL;
};