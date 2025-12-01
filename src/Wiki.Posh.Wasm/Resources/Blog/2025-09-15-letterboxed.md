---
title: I got bored and solved LetterBoxed programmatically
---

For those who might not know, LetterBoxed is a game owned by the New York Times in which 12 pseudorandom letters appear in a square shape, 3 on each side. The goal of the game is to touch each letter at least once by forming as few words as possible by drawing an uninterrupted line (e.g., if your first word ends with "N", your second word must also begin with "N"). You cannot touch the same letter side of the square twice in a row. The game seems to present 4 as a good number of words to complete the game. It seems to be a matter of using the fewest words possible, but beyond that, I decided to define my success criteria as being able to find the lowest number of words required to solve a given problem, and then within that, the shortest possible solution in terms of number of characters. 

I decided to write the solver in C using vim, because I'm an insane masochist. I've never really used C before, but that's fine, because I have this thing called *the sheer fucking audacity*, which gives me the power to fail spectacularly at whatever I set my mind to until I either happen upon a half-working solution or manage to brick my system. 

The first thing I needed in order to solve my problem was a list of valid words. I couldn't find any solid answer for the word list used, so I decided to use dwyl/english-words@20f5cc9's words.txt file. There's actually a lot of efficiency to be gained by refining this input before even compile time: I used grep to remove any words that were not comprised of the 52 uppercase and lowercase Latin alphabet characters, as well as anything shorter than 4 characters, because they're not valid answers. This left me with 411,336 of the 466,550 entries. I then used awk to transform them all to lowercase, to save from doing so at runtime. 

Next up was reading the file, which was surprisingly simple, once I figured out that `char *a` was a list of chars (string), and `char **a` was a list of list of chars (list of strings). 

```c
char **read_words(int num_lines)  
{     
    const char *filename = "words.txt";   
    FILE *file = fopen(filename, "r");     
    char **lines = malloc(num_lines * sizeof(char *));
    const int max_line_len = 45;
    for (int i = 0; i < num_lines; i++)      
    {         
        lines[i] = malloc(max_line_len);         
        fgets(lines[i], max_line_len, file);         
        lines[i][strcspn(lines[i], "\n")] = '\0';     
    }     
    fclose(file);     
    return lines; 
}
```

Initially I read the available letters from the console, but this interfered with benchmarking, so I just hard coded them, and spent way too long figuring out that I needed to allocate an extra byte for the null terminating character. This is the input for 2025-09-15: 

```c
char all_chars[13] = "tiamwphrlgec";
```

This is where shit started to hit the fan. I needed to split these into groups of 3, and then map each group to the groups it wasn't. In the absence of a map structure or any kind of split method on our "string", I created the elegant and concise: 

```c
  char group1[3];
  group1[0] = all_chars[0];
  group1[1] = all_chars[1];
  group1[2] = all_chars[2];

  char group2[3];
  group2[0] = all_chars[3];
  group2[1] = all_chars[4];
  group2[2] = all_chars[5];

  char group3[3];
  group3[0] = all_chars[6];
  group3[1] = all_chars[7];
  group3[2] = all_chars[8];

  char group4[3];
  group4[0] = all_chars[9];
  group4[1] = all_chars[10];
  group4[2] = all_chars[11];

  char group1_targets[9];
  memcpy(group1_targets, group2, 3);
  memcpy(group1_targets + 3, group3, 3);
  memcpy(group1_targets + 6, group4, 3);

  char group2_targets[9];
  memcpy(group2_targets, group1, 3);
  memcpy(group2_targets + 3, group3, 3);
  memcpy(group2_targets + 6, group4, 3);

  char group3_targets[9];
  memcpy(group3_targets, group1, 3);
  memcpy(group3_targets + 3, group2, 3);
  memcpy(group3_targets + 6, group4, 3);

  char group4_targets[9];
  memcpy(group4_targets, group1, 3);
  memcpy(group4_targets + 3, group2, 3);
  memcpy(group4_targets + 6, group3, 3);
```

It may be noted that I did this every time I needed to check if a word was valid, as opposed to just once and re-using it. The reason I did this is because I am stupid, and the reason I have not fixed it is because I tried, segfaulted, cried, and gave up. 

Anyways, to determine if a word could be made using the available box, I used this brilliantly named method: 

```c
int cuctm_exclude(const char * line,
  const char all_chars[12]) {
  int can_use = 1;
  for (const char * p = line;* p; p++) {
    if (strchr(all_chars, * p) == NULL) {
      can_use = 0;
    }
  }

  // memcpy hell...

  for (int i = 0; line[i + 1] != '\0'; i++) {
    char cur = line[i];
    char next = line[i + 1];
    if (contains(group1, 3, cur) && !contains(group1_targets, 9, next)) {
      can_use = 0;
    } else if (contains(group2, 3, cur) && !contains(group2_targets, 9, next)) {
      can_use = 0;
    } else if (contains(group3, 3, cur) && !contains(group3_targets, 9, next)) {
      can_use = 0;
    } else if (contains(group4, 3, cur) && !contains(group4_targets, 9, next)) {
      can_use = 0;
    }
  }
  return can_use;
}
```

Upon re-reading this, I note that I could have just returned 0 at any point instead of using the variable `can_use`, and it would have been much more efficient. If it bothers you that much, send me a PR and I'll ignore it. 

Next up was figuring out which words were valid next to each other. This was surprisingly simple, though made me realise how much I take `IEnumerable<T>.Count` for granted. 

```c
char ** find_next_words(const char * this_word, char ** targets,
  const int n_targets, int * out_count) {
  size_t word_len = strlen(this_word);
  char last_char = this_word[word_len - 1];
  int n_ret = 0;
  char ** ret = NULL;

  for (int i = 0; i < n_targets; i++) {
    char * target = targets[i];
    if (last_char == target[0]) {
      char ** tmp = realloc(ret, (n_ret + 1) * sizeof(char * ));
      if (!tmp) {
        free(ret);
        * out_count = 0;
        return NULL;
      }

      ret = tmp;
      ret[n_ret] = target;
      n_ret++;
    }
  }

  * out_count = n_ret;
  return ret;
}
```

Then, I wrapped this in a bit of recursion, to find valid word sequences of a given length. This was segfault hell. Also, I couldn't be bothered passing around `char**`, so I just started printing to the console. This will come back to bite me later. 

```c
void find_sequences(char * current, char ** valid_words, int n_valid, int target_n_chars, int depth, int * success) {
  if (depth == 0) {
    if (count_distinct_chars(current) == target_n_chars) {
      printf("%s\n", current);
      * success = 1;
    }
    return;
  }

  size_t current_len = strlen(current);
  char last_char = current[current_len - 1];
  int next_count;
  char ** next_words = find_next_words(current + current_len - strlen(current), valid_words, n_valid, & next_count);

  for (int i = 0; i < next_count; i++) {
    char * next_word = next_words[i];
    size_t new_len = current_len + strlen(next_word) + 2;
    char * new_seq = malloc(new_len);

    if (current_len > 0) {
      snprintf(new_seq, new_len, "%s-%s", current, next_word);
    } else {
      snprintf(new_seq, new_len, "%s", next_word);
      * success = 1;
    }
    find_sequences(new_seq, valid_words, n_valid, target_n_chars, depth - 1, success);
    free(new_seq);
  }
  free(next_words);
}
```

`count_distinct_chars` was also not terrible to implement, though I did end up having to manually exclude `-` on account of my laziness in not returning `char**` earlier. Oops. 

```c
int count_distinct_chars(char * string) {
  int len = strlen(string);
  char * chars = malloc(len * sizeof(char));
  int n_chars = 0;
  for (int i = 0; i < len; i++) {
    char cur = string[i];
    if (cur == '-') 
    {
        continue;
    }

    int do_continue = 0;
    for (int j = 0; j < n_chars; j++) {
      if (chars[j] == cur) {
        do_continue = 1;
      }
    }

    if (do_continue) {
      continue;
    }

    chars[n_chars] = cur;
    n_chars++;
  }

  return n_chars;
}
```

With all this tied together in a horrifying monstrosity of a script you can find on my GitHub, there was one challenge left: sorting them by length ascending. 

With my inability to consistently rename symbols because of shitty IDE support and the fact that I'd already given up on any sort of persistence of the output I deemed valid, I decided to follow the unix philosophy and say that this program's purpose was to find, not sort, valid output. In true unix fashion, I sorted my output with: 

```bash
./out.a | awk '{ print length, $1 }' | sort -nr
```

And the long-awaited output? 

```
Segmentation fault         (core dumped) ./out.a
```

Please now envision, as I cannot describe it in words, me screaming. I swear this worked earlier. I did manage to benchmark it while it was still working, and despite all my horrible inefficiencies, it ran in an average of 0.32 seconds on today's input.

```
OS: Arch Linux on Windows 10 x86_64 
Kernel: 6.6.87.2-microsoft-standard-WSL2 
CPU: Intel Xeon W-1290P (20) @ 3.7GHz 
Memory: 32GB 
Compiler: gcc (GCC) 15.2.1 20250813 (default flags)
Benchmarker: hyperfine 1.19.0, 1000it 

Time (mean ± σ): 319.9 ms ± 107.6 ms [User: 296.7 ms, System: 18.4 ms] 
Range (min … max): 301.9 ms … 3702.0 ms 1000 runs
```

Yes, I'm using Arch WSL on Windows 11, because as previously mentioned, I hate myself.

I'm never using C again. Bye. 