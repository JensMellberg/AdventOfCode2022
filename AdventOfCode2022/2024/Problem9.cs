using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem9 : StringProblem
    {
        public override void Solve(IEnumerable<string> testInput)
        {
            var memory = testInput.Single();
            var memoryParts = new List<MemoryPart>();
            var currentId = 0;
            
            for (var x = 0; x < memory.Length; x++)
            {
                var length = memory[x] - '0';
                if (x % 2 == 0)
                {
                    memoryParts.Add(new MemoryPart { Id = currentId++.ToString(), Length = length });
                }
                else if (length > 0)
                {
                    memoryParts.Add(new MemoryPart { Id = null, Length = length });
                }
            }

            var listCopy = memoryParts.Select(x => x.Copy()).ToList();
            this.RearrangeMemory(memoryParts);
            this.RearrangeMemoryNoPartial(listCopy);
            this.PrintResult(this.CalculateChecksum(memoryParts));
            this.PrintResult(this.CalculateChecksum(listCopy));
        }

        private void RearrangeMemory(List<MemoryPart> memoryParts)
        {
            var firstMemoryPointer = 0;
            var queue = new Queue<(int index, MemoryPart memoryPart)>();
            for (var i = memoryParts.Count - 1; i >= 0; i--)
            {
                if (memoryParts[i].Id != null)
                {
                    queue.Enqueue((i, memoryParts[i]));
                }
            }

            while (queue.Any())
            {
                (var partIndex, var memoryPart) = queue.Dequeue();
                var sizeToFill = memoryPart.Length;
                while (sizeToFill > 0)
                {
                    var emptyIndex = GetFirstMemoryIndex();
                    if (emptyIndex == -1 || emptyIndex > partIndex)
                    {
                        memoryPart.Length = sizeToFill;
                        break;
                    }

                    var emptyMemory = memoryParts[emptyIndex];
                    if (emptyMemory.Length <= sizeToFill)
                    {
                        sizeToFill -= emptyMemory.Length;
                        if (emptyMemory.IsPartial)
                        {
                            emptyMemory.InsertedParts.Add(new MemoryPart { Id = memoryPart.Id, Length = emptyMemory.Length});
                            emptyMemory.Length = 0;
                        }
                        else
                        {
                            emptyMemory.Id = memoryPart.Id;
                        }
                    }
                    else
                    {
                        emptyMemory.IsPartial = true;
                        emptyMemory.InsertedParts.Add(new MemoryPart { Id = memoryPart.Id, Length = sizeToFill });
                        emptyMemory.Length -= sizeToFill;
                        sizeToFill = 0;
                    }

                    //this.Print(string.Join("", memoryParts.Select(x => x.ToString())));
                }

                if (sizeToFill == 0)
                {
                    memoryPart.Id = null;
                }
                
            }

            int GetFirstMemoryIndex()
            {
                for (var i = firstMemoryPointer; i < memoryParts.Count; i++)
                {
                    if (memoryParts[i].Id == null && memoryParts[i].Length > 0)
                    {
                        firstMemoryPointer = i;
                        return i;
                    }
                }

                return -1;
            }
        }

        private void RearrangeMemoryNoPartial(List<MemoryPart> memoryParts)
        {
            var memoryBlocksBySize = new Dictionary<int, List<int>>();
            var queue = new Queue<(int index, MemoryPart memoryPart)>();
            for (var i = memoryParts.Count - 1; i >= 0; i--)
            {
                if (memoryParts[i].Id == null)
                {
                    if (!memoryBlocksBySize.TryGetValue(memoryParts[i].Length, out var list))
                    {
                        list = new List<int>();
                        memoryBlocksBySize[memoryParts[i].Length] = list;
                    }

                    list.Add(i);
                }
                else
                {
                    queue.Enqueue((i, memoryParts[i]));
                }
            }

            while (queue.Any())
            {
                (var partIndex, var memoryPart) = queue.Dequeue();
                var emptyIndex = GetFirstMemoryIndex(memoryPart.Length, partIndex);
                if (emptyIndex == -1)
                {
                    continue;
                }

                var emptyMemory = memoryParts[emptyIndex];
                if (emptyMemory.Length == memoryPart.Length)
                {
                    memoryBlocksBySize[emptyMemory.Length].Remove(emptyIndex);
                    if (emptyMemory.IsPartial)
                    {
                        emptyMemory.Length = 0;
                        emptyMemory.InsertedParts.Add(memoryPart.Copy());
                    } else
                    {
                        emptyMemory.Id = memoryPart.Id;
                    }
                }
                else
                {
                    memoryBlocksBySize[emptyMemory.Length].Remove(emptyIndex);
                    emptyMemory.Length -= memoryPart.Length;
                    emptyMemory.IsPartial = true;
                    emptyMemory.InsertedParts.Add(memoryPart.Copy());
                    
                    memoryBlocksBySize[emptyMemory.Length].Add(emptyIndex);
                }

                memoryPart.Id = null;
                //Print(string.Join("", memoryParts.Select(x => x.ToString()));
            }

            int GetFirstMemoryIndex(int minLength, int maxIndex)
            {
                var minimum = int.MaxValue;
                for (var x = minLength; x < 10; x++)
                {
                    if (memoryBlocksBySize.TryGetValue(x, out var list) && list.Count > 0)
                    {
                        var localMin = list.Min();
                        if (localMin <= maxIndex && localMin < minimum)
                        {
                            minimum = localMin;
                        }
                    }
                }

                return minimum != int.MaxValue ? minimum : -1;
            }
        }

        private long CalculateChecksum(List<MemoryPart> memoryParts)
        {
            long checkSum = 0;
            var actualIndex = 0;
            for (var i = 0; i < memoryParts.Count; i++)
            {
                var memoryPart = memoryParts[i];
                if (memoryPart.IsPartial)
                {
                    foreach (var f in memoryPart.InsertedParts)
                    {
                        var idNumber = int.Parse(f.Id);
                        for (var x = actualIndex; x < actualIndex + f.Length; x++)
                        {
                            checkSum += x * idNumber;
                        }

                        actualIndex += f.Length;
                    }
                }
                else if (memoryPart.Id != null)
                {
                    var idNumber = int.Parse(memoryPart.Id);
                    for (var x = actualIndex; x < actualIndex + memoryPart.Length; x++)
                    {
                        checkSum += x * idNumber;
                    }
                }

                actualIndex += memoryPart.Length;
            }

            return checkSum;
        }

        private class MemoryPart
        {
            public string Id { get; set; }

            public int Length { get; set; }

            public bool IsPartial { get; set; }

            public List<MemoryPart> InsertedParts { get; set; } = new List<MemoryPart>();

            public MemoryPart Copy() => new MemoryPart { Id = Id, Length = Length };

            public override string ToString()
            {
                if (this.Id != null)
                {
                    return new String(this.Id[0], this.Length);
                }

                var memoryPart = new String('.', this.Length);
                return this.IsPartial ? string.Join("", this.InsertedParts.Select(x => x.ToString())) + memoryPart : memoryPart;
            }
        }
    }
}
