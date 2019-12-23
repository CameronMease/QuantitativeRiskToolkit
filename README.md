# QuantitativeRiskToolkit
The Quantitative Risk Toolkit is a library for helping users perform Monte Carlo simulations in C#, most notably for risk analysis.  Some of the goals are to:
- Easily create random samples for select distributions.
- Allow distributions to be used in expressions such as addition, subtraction, multiplication, and division.
- Build models of risk scenarios such as the FAIR ([Factor Analysis of Information Risk](https://www.fairinstitute.org/)) model.
- Provide easy serialization/deserialization of distributions, models, and expressions.
- Be repeatable: A model with the same inputs should always return the same outputs (unless the random seeds are changed).

## State of the Project
I'm very much in an exploratory phase of seeing how things work and trying new things.  This includes learning to use Github, as well.  Changes will be breaking and code may be incomplete for quite some time!

## Build Environment
The software is being developed in Visual Studio 2019 with .NET Core 3.x.
