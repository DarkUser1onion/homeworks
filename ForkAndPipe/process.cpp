#include <unistd.h>
#include <fcntl.h>
#include <filesystem>
#include <vector>
#include <thread>
#include <cstring>
#include <iostream>
// #include <sys/stat.h>  - мало инфы, поэтому я буду использовать костыльный вызов system

int main(int argc, char** argv)
{
    std::filesystem::path pt = "/home/user/";
    std::filesystem::path ptFifo;
    int time;
    std::vector<std::filesystem::path> allFiles;

    ptFifo = argv[1];
    time = atoi(argv[2]);
    
    if(time == 0)
    {
        std::cout << "Неверно введена задержка!" << std::endl;
        exit(1);
    }


    for(auto& paths: std::filesystem::directory_iterator(pt))
    {
        allFiles.push_back(paths);
    }
    
    int fd = open(ptFifo.c_str(), O_WRONLY);

    while(true)
    {
        for(size_t i = 0; i < allFiles.size(); i++)
        {
            std::string cmd = "stat "; cmd += allFiles[i];
            int status = system(cmd.c_str());
            std::string temp(std::to_string(status));
            
            write(fd, temp.c_str(), temp.size());
            std::this_thread::sleep_for(std::chrono::seconds(time));
        }
    }
}
