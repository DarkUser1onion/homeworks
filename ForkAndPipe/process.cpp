#include <unistd.h>
#include <fcntl.h>
#include <filesystem>
#include <vector>
#include <thread>
// #include <sys/stat.h>  - мало инфы, поэтому я буду использовать костыльный вызов system

int main()
{
    std::filesystem::path pt = "/home/user/";
    std::filesystem::path ptFifo = "/tmp/pipe";
    std::vector<std::filesystem::path> allFiles;

    
    for(auto& paths: std::filesystem::directory_iterator(pt))
    {
        allFiles.push_back(paths);
    }
    
    // allFiles.push_back("~/wefwef"); тест аномалии

    int fd = open(ptFifo.c_str(), O_WRONLY);


    while(true)
    {
        for(size_t i = 0; i < allFiles.size(); i++)
        {
            std::string cmd = "stat "; cmd += allFiles[i];
            int status = system(cmd.c_str());
            std::string temp(std::to_string(status));
            
            write(fd, temp.c_str(), temp.size());

            std::this_thread::sleep_for(std::chrono::seconds(2));
        }
    }
}