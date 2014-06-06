using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RollingHash
{
    public class RollingHash
    {
        // fix this to 32 bits
        private int BarrelShiftMSB = 0x8000;

        private string InputText;

        private int[] KeySizes;
        private string[] Keys;
        private int[] KeyHash;
        private int[] KeyToComplareHash;

        public RollingHash(string inputText)
        {
            this.InputText = inputText;
        }

        public int BarrelShift(char input, int k)
        {
            int bitShift;
            int returnShiftedChar = input;
            while (k-- != 0)
            {
                bitShift = input & this.BarrelShiftMSB;
                returnShiftedChar = ((int)input << 1) | bitShift;
            }

            return returnShiftedChar;
        }

        public void FreshHash()
        {
            int textStartPosition = 0;

            // For each key given to complate the text to, calculate the hash of that key
            for (int keyIndex = 0; keyIndex < this.KeySizes.Length && this.KeySizes[keyIndex] + textStartPosition <= this.InputText.Length; keyIndex++)
            {
                if (this.KeySizes[keyIndex] + textStartPosition <= this.InputText.Length)
                {
                    this.KeyHash[keyIndex] = this.RollHash(0, this.KeySizes[keyIndex]);
                }
                else
                {
                    this.KeyHash[keyIndex] = this.KeyHash[keyIndex - 1];
                }
            }
        }

        public int RollHash(int starting, int size)
        {
            int hashValue = 0;
            while (size != 0)
            {
                hashValue ^= this.BarrelShift(this.InputText[starting], size - 1);
                size--;
                starting++;
            }

            return hashValue;
        }

        public int RollHash(string input)
        {
            int hashValue = 0;
            for(int index = 0; index < input.Length; index++)
            {
                hashValue ^= this.BarrelShift(input[index], input.Length - (index+1));
            }

            return hashValue;
        }

        public int NextRollingHash(int currentHash, char outChar, char inChar, int size)
        {
            int hashValue = currentHash ^ this.BarrelShift(outChar, size) ^ (int)inChar;
            return hashValue;
        }

        public int HashNext(int currentHash, int startPosition, int keySize, int nextTimes)
        {
            int hashValue = currentHash;

            while (nextTimes != 0 && startPosition + nextTimes < this.InputText.Length)
            {
                hashValue = this.NextRollingHash(hashValue, this.InputText[startPosition], this.InputText[startPosition + keySize], keySize);
                nextTimes--;
                startPosition++;
            }

            return hashValue;
        }

        public bool ContainsKeys(string[] keys)
        {
            int[] keySizes = new int[keys.Length];
            for (int index = 0; index < keys.Length; index++)
            {
                keySizes[index] = keys[index].Length;
            }
            Array.Sort(keySizes);
            this.KeySizes = keySizes;
            this.KeyHash = new int[keys.Length];
            this.KeyToComplareHash = new int[keys.Length];
            bool found = false;
            int startPosition = 0;

            // Calculate a fresh hash for all keys
            for (int index = 0; index < keys.Length; index++)
            {
                this.KeyToComplareHash[index] = this.RollHash(keys[index]);
            }
            this.FreshHash();
            // While the first size + start position < inputTextSize && notFound
            while (this.KeySizes[0] + startPosition < this.InputText.Length && found == false)
            {
                // Hash next all the keys
                for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                {
                    if (this.KeyToComplareHash[keyIndex] == this.KeyHash[keyIndex])
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        this.KeyHash[keyIndex] = HashNext(this.KeyHash[keyIndex], startPosition, this.KeySizes[keyIndex], 1);
                    }
                }
                startPosition++;
            }

            return found;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            RollingHash rh = new RollingHash("This is a really big text that you need to search inside.");
            rh.ContainsKeys(new string[] {"is"});
        }
    }
}
