// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// Using directive should appear within a namespace declaration | https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1200.md
#pragma warning disable SA1200

using AsepriteTesting;

// Create a new game instance and run it to start the game
var game = new Game();
game.Show();
