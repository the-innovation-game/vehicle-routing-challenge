# The Innovation Game - Vehicle Routing Challenge

## Objective
**Capacitated Vehicle Routing Problem** is a combination of the classical Travelling Salesman Problem and the Knapsack Problem. 

The challenge is to develop an algorithm that finds routes which do not exceed the vehicle capacity and have a total distance at least 15% shorter than a naive greedy algorithm.

## Repo Structure

* `VehicleRouting.Algorithms` contains all the uploaded algorithms for this challenge. Any algorithm you develop should go in here
* `VehicleRouting.Challenge` contains the logic for running & verifying algorithms for this challenge
* `VehicleRouting.Runner` contains an executable program for debugging / running your algorithms during development
* `VehicleRouting.Tests` contains tests for the challenge logic 

## Getting Started

1. [Install Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

2. Fork and clone this repo

3. Create a branch for your algorithm
```
git checkout -b <team name>/algorithm/<algorithm name>
```

3. Open `VehicleRouting.sln` with Visual Studio 2022

4. Set `VehicleRouting.Runner` as your startup project

![](assets/set-startup-project.png)

5. Make a copy of `VehicleRouting.Algorithms\Template.cs` and rename filename & class to your algorithm name

![](assets/my-first-algo.png)

6. Modify `VehicleRouting.Runner\Program.cs` to use your algorithm and start developing / debugging!

![](assets/start-debugging.png)

7. During the allowed window, push up your branch and open a pull request to merge your branch to the master repository 

## Support
[Join our Discord](http://discord.the-innovation-game.com)
