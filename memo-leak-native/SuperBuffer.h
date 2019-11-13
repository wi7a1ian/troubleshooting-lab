#pragma once
#include <memory>

template <typename T>
struct SuperBuffer
{
	void Reset(size_t size)
	{
		mybuff = std::make_unique<T[]>(size);
	}

	T* Get()
	{
		return mybuff.release();
	}

	std::unique_ptr<T[]> mybuff;
};