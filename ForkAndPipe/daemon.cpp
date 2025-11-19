#include <chrono>
#include <thread>
#include <unistd.h>
#include <fcntl.h>
#include <sys/stat.h>
#include <sys/wait.h>
#include <filesystem>
#include <signal.h>
#include <iostream>

pid_t pid = -1;

void exiter(int sig)
{
    if(pid > 0)
    {
        kill(pid, 15);
        std::this_thread::sleep_for(std::chrono::seconds(2));

        if(waitpid(pid, nullptr, WNOHANG) == 0)
        {
            kill(pid, 9);
            waitpid(pid, nullptr, WNOHANG);
        }
    }

    std::cout
    << "\nНа child процесс был отправлен сигнал {"
    << sig
    << "}, он был уничтожен!"
    << std::endl;

    pid = -1;
}

int main(int argc, char** argv)
{
    std::filesystem::path pt = "/tmp/pipe";
    
    if(!std::filesystem::exists(pt))
    {
        mkfifo(pt.c_str(), 0644);
    }
    
    while(true)
    {
        pid = fork();
        if(pid < 0)
        {
            std::cout << "Ошибка fork'a";
            exit(1);
        }
        else if(pid == 0)
        {
            std::cout << "Запуск дочернего процесса!" << std::endl;
            execlp("./process", "process", nullptr);
        }
        else
        {
        
            int fd = open(pt.c_str(), O_RDONLY);
        
            signal(15, exiter);
            
            std::string buf(1, '\0'); // костыль для if
            ssize_t bufSize;
            while((bufSize = read(fd, buf.data(), buf.size())) > 0)
            {
                write(1, buf.data(), buf.size());

                if(waitpid(pid, nullptr, WNOHANG) != 0)
                {
                    break;
                }

                if(buf != "0")
                {
                    std::cout << "\nОбнаружена аномалия, перезапускаем процесс!" << std::endl;
                    exiter(15);
                    std::this_thread::sleep_for(std::chrono::seconds(2));
                    if(waitpid(pid, nullptr, WNOHANG) == 0)
                    {
                        std::cout << "\nAnti-forkbomb вырубает родительский процесс!" << std::endl;
                        exit(1);
                    }
                    break;
                }
            }

        }
    }
}
