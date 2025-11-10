#include "sha256.h"
#include <vector>
#include <unistd.h>
#include <fcntl.h>
#include <cstring>
#include <iostream>
#include <string>
 
int main()
{
	std::vector<char*> passwords;

	std::cout << "Parsing rockyou.txt...";

	int fd = open("rockyou.txt", O_RDONLY);
	if(fd == -1)
	{
		std::cout << "Неверно указан файл rockyou.txt или его не существует";
		return 255;
	}

	std::vector<char> buffer(1024);
	std::vector<char*> temp;

	size_t dump;

	while((dump = read(fd, buffer.data(), buffer.size())) > 0)
	{
		passwords.push_back(buffer.data());
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
				std::cout << "Пароль найден! {" << passwords[i] << "}, его hash: {" << sha256(passwords[i]) << "}" << std::endl;
				flag = false;
				pause();
				break;
			}
			else
			{
				write(1, "Пробую пароль #", 15);
				write(1, std::to_string(counter).c_str(), std::to_string(counter).size());
				write(1, " {", 2);
				write(1, passwords[i], strlen(passwords[i]));
				write(1, " }\n", 4);
			}
			
			counter++;
		}

		if(flag)
		{
			std::cout << "Пароль не найден!" << std::endl;
			pause();
		}
	}

	return 0;
}
