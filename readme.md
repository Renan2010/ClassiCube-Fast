ClassiCube is a custom Minecraft Classic compatible client written in C from scratch.<br>
**It is not affiliated with (or supported by) Mojang AB, Minecraft, or Microsoft in any way.**

# Classicube-Fast
This is a classicube compiled with clang and with aggressive game optimizations for the best possible performance
## How to compile this project
**Clone the repository**
  ```bash
   $ git clone https://github.com/Renan2010/ClassiCube-Fast.git
  ```
**Enter to directory**
  ```bash
   $ cd ClassiCube-Fast
  ```
**Compile (Note:all optimizations are already active)**
  ```bash
   $ CC=clang make -j $(nproc)
  ```
**And enjoy :)**
# FAQ
**What is "$(nproc)"?**

"$(nproc)" Are the total number of cores on your Machine/PC. Example: My i5-2400 4C/4T
  ```bash
  $ echo $(nproc) # Output the total number of cores in the machine
  Output: 4
  ```
