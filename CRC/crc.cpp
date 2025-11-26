#include <iostream>
#include <cstdint>

uint8_t crc8(uint8_t* data, size_t len)
{
	uint8_t crc = 0;
	for(size_t i = 0; i < len; i++)
	{
		crc ^= data[i];
		for(int j = 0; j < 8; j++)
		{
			if(crc & 0x80)
			{
				crc <<= 1;
				crc ^= 0xD5;
			}
			else
				crc <<= 1;
		}
	}
	return crc;
}

uint16_t crc16(uint8_t* data, size_t len) {
    uint16_t crc = 0;
    for(size_t i = 0; i < len; ++i) {
        crc ^= static_cast<uint16_t>(data[i]) << 8;
        for(int j = 0; j < 8; ++j) {
            if(crc & 0x8000) {
                crc <<= 1;
                crc ^= 0xA001;
            }
            else {
                crc <<= 1;
            }
        }
    }
    return crc;
}

const uint8_t crc8_table[] = {
    0x00, 0xF5, 0xEA, 0x1F, 0xE9, 0x1C, 0x07, 0xFC
};

const uint16_t crc16_table[] = {
    0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241
};

uint8_t crc8_tab(uint8_t* data, size_t len) {
    uint8_t crc = 0;
    for(size_t i = 0; i < len; ++i) {
        crc = crc8_table[(crc ^ data[i])];
    }
    return crc;
}

uint16_t crc16_tab(uint8_t* data, size_t len) {
    uint16_t crc = 0;
    for(size_t i = 0; i < len; ++i) {
        crc = (crc >> 8) ^ crc16_table[(crc & 0xFF) ^ data[i]];
    }
    return crc;
}

int main()
{
	const char test_data[] = "Test string";

	std::cout << "CRC-8 (битовый сдвиг): " << std::hex << int(crc8((uint8_t*)test_data, sizeof(test_data)-1)) << std::endl;
	std::cout << "CRC-8 (табличный метод): " << std::hex << int(crc8_tab((uint8_t*)test_data, sizeof(test_data)-1)) << std::endl;

	std::cout << "CRC-16 (битовый сдвиг): " << std::hex << crc16((uint8_t*)test_data, sizeof(test_data)-1) << std::endl;
	std::cout << "CRC-16 (табличный метод): " << std::hex << crc16_tab((uint8_t*)test_data, sizeof(test_data)-1) << std::endl;
}
