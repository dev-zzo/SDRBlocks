
Sources and Sinks

Generally, there are two major types of S&Ses:
* Synchronous -- pushing/pulling data at a constant rate (soundcard, network devices, visualisers)
* Asynchronous -- which can produce data on demand (generators, file sources)

Need to think of a way to allow those to cooperate nicely.
For now:
* We won't have async inputs
* Any generator will be built-in into blocks
