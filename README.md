<p align="center">
<a href="https://neo.org/">
      <img
      src="https://neo3.azureedge.net/images/logo%20files-dark.svg"
      width="250px" alt="neo-logo">
  </a>
</p>

<p align="center">
  <a href='https://coveralls.io/github/neo-project/neo-devpack-dotnet'>
    <img src='https://coveralls.io/repos/github/neo-project/neo-devpack-dotnet/badge.svg' alt='Coverage Status' />
  </a>
  <a href="https://github.com/neo-project/neo-devpack-dotnet/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg">
  </a>
</p>

# NEO Devpack Dotnet

## Overview
This is the set of development tools for .NET.

For detailed documentation, see [docs.neo.org](https://docs.neo.org/docs/en-us/index.html) and the [NEO whitepaper](https://docs.neo.org/docs/en-us/basic/whitepaper.html).



## Project structure
An overview of the project folders can be seen below.

|Folder|Content|
|---|---|
|Neo.Compiler.MSIL| Classes used to compile C# codes to NeoVM opcodes.|
|Neo.SmartContract.Framework|The framework of Neo SmartContract. Neo.SmartContract.Framework.SmartContract Is the default superclass of smart contract.|
|templates|The template smart contracts of C#, VB and NEP5. You can refer to them to write your own smart contract|

## How to use it

For compiling a contract, see [Compiling a contract sample.](https://docs.neo.org/v3/docs/en-us/sc/gettingstarted/develop.html)


## Related projects

* [neo:](https://github.com/neo-project/neo/) Neo core library, contains base classes, including ledger, p2p and IO modules.
* [neo-vm:](https://github.com/neo-project/neo-vm/) Neo Virtual Machine is a decoupled VM that Neo uses to execute its scripts. It also uses the `InteropService` layer to extend its functionalities.
* [neo-node:](https://github.com/neo-project/neo-node/) Executable version of the Neo library, exposing features using a command line application or GUI.
* [neo-modules:](https://github.com/neo-project/neo-modules/) Neo modules include additional tools and plugins to be used with Neo.