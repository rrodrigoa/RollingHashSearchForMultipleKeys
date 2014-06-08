using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RollingHash
{
    public class RollingHash
    {
        // fix this to 32 bits
        private int BarrelShiftMSB = 1 << 31;

        private string InputText;

        private int[] KeySizes;
        private string[] Keys;
        private int[] KeyHash;
        private int[] KeyToComplareHash;
        private int inputLegth;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RollingHash(string inputText)
        {
            this.InputText = inputText;
            this.inputLegth = inputText.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BarrelShift(char input, int k)
        {
            int bitShift;
            int returnShiftedChar = input;
            while (k-- != 0)
            {
                bitShift = input & this.BarrelShiftMSB;
                returnShiftedChar = ((int)returnShiftedChar << 1) | bitShift;
            }

            return returnShiftedChar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BarrelShift(int hashKey)
        {
            int bitShift = hashKey & this.BarrelShiftMSB;
            int returnShiftedHashKey = hashKey << 1 | bitShift;
            return returnShiftedHashKey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FreshHash()
        {
            int textStartPosition = 0;

            // For each key given to complate the text to, calculate the hash of that key
            for (int keyIndex = 0; keyIndex < this.KeySizes.Length && this.KeySizes[keyIndex] + textStartPosition <= this.inputLegth; keyIndex++)
            {
                if (this.KeySizes[keyIndex] + textStartPosition <= this.inputLegth)
                {
                    this.KeyHash[keyIndex] = this.RollHash(0, this.KeySizes[keyIndex]);
                }
                else
                {
                    this.KeyHash[keyIndex] = this.KeyHash[keyIndex - 1];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            for (int index = 0; index < input.Length; index++)
            {
                hashValue ^= this.BarrelShift(input[index], input.Length - (index + 1));
            }

            return hashValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextRollingHash(int currentHash, char outChar, char inChar, int size)
        {
            int hashValue = this.BarrelShift(currentHash) ^ this.BarrelShift(outChar, size) ^ (int)inChar;
            return hashValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int HashNext(int currentHash, int startPosition, int keySize, int nextTimes)
        {
            int hashValue = currentHash;
            while (nextTimes != 0 && startPosition + nextTimes < this.inputLegth)
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
            while (this.KeySizes[0] + startPosition < this.inputLegth && found == false)
            {
                // Hash next all the keys
                for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                {
                    if (this.KeyToComplareHash[keyIndex] == this.KeyHash[keyIndex] && InputText.Substring(startPosition, this.KeySizes[keyIndex]) == keys[keyIndex])
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        if (startPosition + this.KeySizes[keyIndex] < this.inputLegth)
                        {
                            this.KeyHash[keyIndex] = HashNext(this.KeyHash[keyIndex], startPosition, this.KeySizes[keyIndex], 1);
                        }
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
            string inputText = "Of course I’m also interested in a K4W2 hackathon and I think we would all love to see your demo. In the area of UI and Kinect gestures I think that we could brainstorm some affordances that can be built into future UIs. Through experimentation (some of which I’m sure the Kinect team has done already) we can build UIs that are simple, intuitive, responsive, and robust. I like what our design team has done in terms of making our modern interfaces distinctive and recognizable, but they are made at the cost of usability and discoverability. Interaction should be a two way conversation between the user and the machine/interface. Donald Norman outlines these principles in his books and papers. Things like affordances mapping current state, actions available to the user, robustness against errors, speed and performance, simplicity. It would be neat to first break down the k4wv2 data into primitive components (x, y, distance of hands, hand gesture, etc. then build a second layer on top of of these primitives to add things like linear velocity, inertia, maybe rotational ballistics with two hands, distance between hands, build discrete and continuous gestures on top of these.  Then build the UI affordances that help and teach users to navigate the UI.Also it would be powerful to see K4W2 products from scenarios like 3D object capture, texture mapping /scanning, and some of the more advanced Kinect projects shown at tech fest. CF Sent from Windows Mail ";
            string key1 = "bicloqi283.com";
            string key2 = "flksajldsiikgiyguyfutcutcxytxcyxcyrdtrezrezresa65rf87hoiuhe.com";
            string key3 = "llblii39390.com";
            string key4 = "ll;q;qpopowei.com";

            Stopwatch watch = new Stopwatch();

            watch.Start();
            watch.Stop();
            watch.Restart();

            RollingHash rh = new RollingHash(inputText);
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            rh.ContainsKeys(new string[] { key1, key2, key3, key4 });
            watch.Stop();            

            Console.WriteLine("Milliseconds for rolling hash = " + watch.ElapsedTicks);
            
            watch.Restart();
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            inputText.IndexOf(key1); inputText.IndexOf(key2); inputText.IndexOf(key3); inputText.IndexOf(key4);
            watch.Stop();

            Console.WriteLine("Milliseconds for IndexOf = " + watch.ElapsedTicks);

            watch.Restart();
            Regex reg = new Regex("(" + key1 + "|" + key2 + "|" + key3 + "|" + key4 + ")");
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            reg.Match(inputText);
            watch.Stop();

            Console.WriteLine("Milliseconds for Regex = " + watch.ElapsedTicks);
        }
    }
}
