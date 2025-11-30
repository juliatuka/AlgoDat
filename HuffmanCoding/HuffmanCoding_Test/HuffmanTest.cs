using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuffmanCoding;

namespace HuffmanCoding_Test
{
	[TestFixture]
	public class HuffmanTest
	{
		private string tempDir;
		private string inputPath;
		private string encodePath;
		private string decodePath;
		private HuffmanCoder huffman;

		[SetUp]
		public void Setup()
		{
			huffman = new HuffmanCoder();

			tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDir);

			inputPath = Path.Combine(tempDir, "input.txt");
			encodePath = Path.Combine(tempDir, "encoded.huf");
			decodePath = Path.Combine(tempDir, "decoded.txt");
		}

		[TearDown]
		public void Cleanup()
		{
			if (Directory.Exists(tempDir))
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[Test]
		public void Encoding_And_Decoding_SimpleText_Returns_Original()
		{
			string original = "hello huffman\nthis is a test";
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			huffman.Encode(inputPath, encodePath);

			huffman.Decode(encodePath, decodePath);

			string decoded = File.ReadAllText(decodePath, Encoding.UTF8);
			Assert.That(original, Is.EqualTo(decoded));
		}

		[Test]
		public void Encoding_And_Decoding_Single_CharacterRepeated_Returns_Original()
		{
			string original = new string('x', 20);
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			huffman.Encode(inputPath, encodePath);
			huffman.Decode(encodePath, decodePath);

			string decoded = File.ReadAllText(decodePath, Encoding.UTF8);
			Assert.That(original, Is.EqualTo(decoded));
		}

		[Test]
		public void Encoding_And_Decoding_With_Whitespace_And_Unicode()
		{
			string original = "Huffman coding: äöü ß – test, 123!\nNew line.";
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			huffman.Encode(inputPath, encodePath);
			huffman.Decode(encodePath, decodePath);

			string decoded = File.ReadAllText(decodePath, Encoding.UTF8);
			Assert.That(original, Is.EqualTo(decoded));
		}

		[Test]
		public void Encoding_Writes_Valid_HuffmanFileHeader_And_Table()
		{
			string original = "abaac";
			File.WriteAllText(inputPath, original, Encoding.UTF8);

			huffman.Encode(inputPath, encodePath);
			string[] lines = File.ReadAllLines(encodePath);

			Assert.That(lines.Length, Is.GreaterThanOrEqualTo(5)); // (1) HF, (2) symbolCount, (3) symbol and frequency..., (4) originalLength, (5) encodedBits
			Assert.That(lines[0], Is.EqualTo("HF"));

			int symbolCount;
			Assert.That(int.TryParse(lines[1], out symbolCount));
			Assert.That(symbolCount, Is.GreaterThan(0));

			var expectedFreq = original.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

			var tableLines = lines.Skip(2).Take(symbolCount).ToArray();

			Assert.That(expectedFreq.Count, Is.EqualTo(tableLines.Length));

			var parsedFreq = new Dictionary<char, int>();
			foreach (var line in tableLines)
			{
				var split = line.Split(',');
				Assert.That(split.Length, Is.EqualTo(2));

				int symbolCode;
				Assert.That(int.TryParse(split[0], out symbolCode));
				char ch = (char)symbolCode;

				int freq;
				Assert.That(int.TryParse(split[1], out freq));

				parsedFreq[ch] = freq;
			}

			foreach (var kv in expectedFreq)
			{
				Assert.That(parsedFreq.ContainsKey(kv.Key));
				Assert.That(kv.Value, Is.EqualTo(parsedFreq[kv.Key]));
			}

			int originalLength;
			Assert.That(int.TryParse(lines[2 + symbolCount], out originalLength));
			Assert.That(original.Length, Is.EqualTo(originalLength));

			string bits = lines[3 + symbolCount];
			Assert.IsNotEmpty(bits);
			Assert.That(bits.All(c => c == '0' || c == '1'));

		}

		[Test]
		public void Encoding_EmptyInputFile_Throws_InvalidOperationException()
		{
			File.WriteAllText(inputPath, string.Empty, Encoding.UTF8);

			Assert.Throws<InvalidOperationException>(() =>
			{
				huffman.Encode(inputPath, encodePath);
			});
		}

		[Test]
		public void Decoding_NonExistingInputFile_Throws_IO_and_FileNotFoundException()
		{
			string nonExisting = Path.Combine(tempDir, "does_not_exist.huf");

			var ex = Assert.Throws<IOException>(() =>
			{
				huffman.Decode(nonExisting, decodePath);
			});

			Assert.IsInstanceOf<FileNotFoundException>(ex.InnerException);
		}

		[Test]
		public void Decoding_InvalidHeader_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath,
			[
				"NOT_HF",
				"0"
			]);

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});

			StringAssert.Contains("missing 'HF' header", ex.Message);
		}

		[Test]
		public void Decoding_InvalidSymbolCount_NotInt_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"NaN"
			});

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});

			StringAssert.Contains("could not read symbol count", ex.Message);
		}

		[Test]
		public void Decoding_Unexpected_EndOfFileInFrequencyTable_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"3",            
                "65,1",       
                "5",         
                "0"      
            });

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_Invalid_FrequencyLineFormat_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				"this_is_wrong_line",
				"5",
				"0"
			});

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_Invalid_SymbolCode_NotInt_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				"A,3",
                "5",
				"0"
			});

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_Invalid_Frequency_NotInt_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				"65,NaN",         
                "5",
				"0"
			});

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_InvalidOriginalLengthNotInt_ThrowsInvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				"65,1",
				"notInt",
				"0"
			});

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_Missing_EncodedBitstring_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				"65,1",
				"1"      
            });

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_Frequency_LessOrEqualZero_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				$"{(int)'A'},0",
				"0",
				"" 
            });

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_Invalid_BitCharacter_Throws_InvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				$"{(int)'A'},1",
				"1",
				"2"  
            });

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_TryGoRightOnNullNode_ThrowsInvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				$"{(int)'A'},1",
				"1",
				"1"
			});

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}

		[Test]
		public void Decoding_DecodedLengthMismatch_ThrowsInvalidDataException()
		{
			File.WriteAllLines(encodePath, new[]
			{
				"HF",
				"1",
				$"{(int)'A'},1",
				"2",   // original length 
                "0"    // data decode
            });

			var ex = Assert.Throws<InvalidDataException>(() =>
			{
				huffman.Decode(encodePath, decodePath);
			});
		}
	}
}
