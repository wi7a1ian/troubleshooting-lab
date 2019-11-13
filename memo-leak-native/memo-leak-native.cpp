// memo-leak-native.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <string_view>
#include "SuperBuffer.h"

int main()
{
	constexpr std::string_view dummyText = "dummy text";
	const int enter = 0x0A;

	while (std::cin.get() == enter)
	{
		SuperBuffer<char> buff;

		std::cout << "Requsting new buffer\n";
		buff.Reset(1'000'000);

		std::cout << "Retrieving buffer\n";
		auto b = buff.Get();

		std::copy(dummyText.cbegin(), dummyText.cend(), b);
		std::cout << b;
	}
}
