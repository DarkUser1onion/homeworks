#include <iostream>
#include <unistd.h>
#include <vector>


int main(int argc, char** argv)
{
    if(argc <= 1)
    {
        std::cout << "Usage: xargs {command} \"enter\" {input files}";
        return 1;
    }

    std::string command;
    std::vector<std::string> files;
    for(int i = 1; i < argc; i++)
    {
        command += argv[i]; command += " ";
    }

    std::string str;

    while(true)
    {
        getline(std::cin, str);

        if(str.size() == 0)
            break;
        else
            files.push_back(str);
    }

    std::cout << std::endl;
    for(size_t i = 0; i < files.size(); i++)
    {
        std::string temp = command + files[i];
        const char* cmd = temp.c_str();
        std::cout << files[i] << ":" << std::endl;
        system(cmd);
    }

    return 0;
}
