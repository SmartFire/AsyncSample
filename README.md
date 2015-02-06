AsyncSample
===========

C#.Net Asynchronous methods, progress reporting, cancelling.

The master enables fine-grained sharing of resources (cpu, ram, ...) across applications by making them *resource offers*. Each resource offer contains a list of \<slave ID, resource1: amount1, resource2, amount2, ...\>.  The master decides *how many* resources to offer to each framework according to a given organizational policy, such as fair sharing, or strict priority. To support a diverse set of policies, the master employs a modular architecture that makes it easy to add new allocation modules via a plugin mechanism.
