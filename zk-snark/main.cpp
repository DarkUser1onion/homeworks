#include <iostream>
#include <string>
#include <vector>
#include <random>
#include <map>
#include <memory>

struct Proof
{
    std::string commitment;
    std::string response;
    int challenge;
};

class SimpleAgeProver
{
private:
    int secret_age;
    
    std::mt19937 rng; // это типа крутой рандом
    
public:
    SimpleAgeProver(int age) : secret_age(age)
    {
        std::random_device rd;
        rng.seed(rd());
    }
    
    std::string create_commitment()
    {
        std::uniform_int_distribution<int> dist(1000, 9999);
        int random_salt = dist(rng);
        
        // в реальном zk snark здесь должны быть сложные махинации с криптографией
        return "commit_" + std::to_string(secret_age) + "_" + std::to_string(random_salt);
    }
    
    std::string create_response(int challenge)
    {
        // имитация что возраст < 18
        if (secret_age < 18)
        {
            return "valid_proof_age_" + std::to_string(secret_age) + "_challenge_" + std::to_string(challenge);
        } 
        else
        {
            return "invalid_proof";
        }
    }
    
    Proof generate_proof()
    {
        Proof proof;
        proof.commitment = create_commitment();
        
        // случайный вызов от проверяющего
        std::uniform_int_distribution<int> dist(1, 100);
        proof.challenge = dist(rng);
        
        proof.response = create_response(proof.challenge);
        return proof;
    }
};

class SimpleAgeVerifier
{
public:
    // проверка доказательства
    bool verify_proof(const Proof& proof)
    {
        if (proof.response.find("valid_proof_age_") != std::string::npos) // npos - индификатор для size_t или "пустоты"
        {
            // извлечение возраста из ответа (в реальной системе это было бы скрыто)
            size_t pos = proof.response.find("age_");
            if (pos != std::string::npos)
            {
                int age = std::stoi(proof.response.substr(pos + 4));
                std::cout << "Скрытый возраст: " << age << " лет\n";
                
                if (age < 18)
                {
                    std::cout << "Принятто\n";
                    return true;
                }
            }
        }
        
        std::cout << "Отклонено\n";
        return false;
    }
};

class TrustedSetup
{
private:
    std::string public_parameters;
    
public:
    TrustedSetup()
    {
        public_parameters = "trusted_params_v1";
    }
    
    std::string get_parameters() const
    {
        return public_parameters;
    }
};

int main()
{
    TrustedSetup setup;
    
    std::vector<std::pair<std::string, int>> test_cases =
    {
        {"Андрей", 16},
        {"Матвей", 25},
        {"Топлес", 17}
    }; // проверка 
    
    for (const auto& test_case : test_cases)
    {
        std::string name = test_case.first;
        int age = test_case.second;
        
        std::cout << "\n--- Тест: " << name << " (" << age << " лет) ---\n";
        
        SimpleAgeProver prover(age);
        Proof proof = prover.generate_proof();
        
        SimpleAgeVerifier verifier;
        bool result = verifier.verify_proof(proof);
        
        std::cout << "Результат для " << name << ": " 
                  << (result ? "ДОСТОВЕРНО" : "НЕДОСТОВЕРНО") << "\n";
    }
    return 0;
}
