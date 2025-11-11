#include "sha256.h"
#include <vector>
#include <unistd.h>
#include <fcntl.h>
#include <cstring>
#include <iostream>
#include <string>
#include <fstream>
 
int main()
{
	std::vector<std::string> passwords;

	std::cout << "Parsing rockyou.txt...\n";

	std::ifstream file("rockyou.txt");

	if(!file.is_open())
	{
		std::cout << "Not correct file rockyou.txt or it doesn't exist";
		return 255;
	}

	
	std::string dump;
	while(std::getline(file, dump))
	{
		passwords.push_back(dump);
	}

	std::string userPass;
	std::string userPassSha;
	
	std::string tempSha;
	bool flag;

	size_t counter = 1;
	while(true)
	{
		flag = true;
		system("clear");
		std::cout << "Enter you pass: ";
		getline(std::cin, userPass);
				
		userPassSha = sha256(userPass);
		
		for(size_t i = 0; i < passwords.size(); i++)
		{
			if(sha256(passwords[i]) == userPassSha)
			{
				std::cout << "Pass found! {" << passwords[i] << "}, hash: {" << sha256(passwords[i]) << "}" << std::endl;
				flag = false;
				pause();
				break;
			}
			else
			{
				write(1, "Trying pass ", 12);
				write(1, std::to_string(counter).c_str(), std::to_string(counter).size());
				write(1, " {", 2);
				write(1, passwords[i].c_str(), passwords[i].size());
				write(1, "}\n", 3);
			}
			
			counter++;
		}

		if(!flag)
		{
			std::cout << "Pass not found!" << std::endl;
			pause();
		}
	}

	return 0;
}
