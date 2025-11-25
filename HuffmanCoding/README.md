# Huffman Coding – File Compressor / Decompressor

This project implements **Huffman Coding**.
It supports encoding and decoding of text files and uses a readable `.huf` format that stores all information needed to rebuild the Huffman Tree.  

Note: The `.huf` extension is recommended for compressed files, but not required.
The program relies on the internal "HF" header to validate Huffman files.

## Usage:

### Encoding:
````
HuffmanCoding.exe encode <input_file> <output_file>
````
Example: 
````
HuffmanCoding.exe encode text.txt compressed.huf
````
### Decoding:
````
HuffmanCoding.exe decode <input_file> <output_file>
````
Example: 
````
HuffmanCoding.exe decode compressed.huf restored.txt
````

## `.huf` File Format:

Line 1: HF \
Line 2: Number of different symbols \
Line 3..n: Symbol Code (integer value of the character), frequency \
Next Line: Length of the original text \
Last Line: Encoded bits 

### Example (for input "aaabbc"):

HF \
3 \
97,3 \
98,2 \
99,1 \
6 \
000111110 

This information fully reconstructs the Huffman tree during decoding.

## Limitations
- Encoded bitstring is stored as text ("0" and "1") → not binary efficient  
- Designed mainly for text files, not raw binary files  
