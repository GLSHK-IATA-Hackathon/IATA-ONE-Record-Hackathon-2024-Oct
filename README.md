## Inspiration

The operation of the air cargo industry lacks transparency in service delivery. 

Key Problems to Address: 

1. Communication Breakdowns: During disruptive events, ineffective communication can result in discrepancies that adversely affect shipment performance. 

2. Increased Discrepancies: Poor coordination and communication can lead to a rising number of discrepancies, undermining operational efficiency and jeopardizing timely deliveries. 

3. Lack of Performance Assessment Tools: Currently, there is no digital tool to compare expected milestone times with actual performance, making it difficult to assess GHA Service Level Agreement (SLA) performance.  

## Solution and what it does 

**Development of a Comprehensive Communication System**

- Includes a mobile app and website tailored for Ground Handling Agents (GHAs) and airlines to enhance communication, streamline resource reallocation, and proactively manage potential delays. 

**Centralized Shipment Information**

- Retrieves shipment information from centralized repository (1R), and incorporates the Cargo IQ concept to compare planned and actual milestone event times. 

**Real-Time Updates**

- Frontline ground staff can use the EzyHandle mobile app to update shipment milestones in real time. 

**Flight Movement Tracking** 

- Integrates flight movement data from airlines and GHA to identify delays promptly. 

**Management of Delayed Milestones and Proactive Handling Suggestions**

- Highlights delayed milestones and recommends workaround 

## How did we build it? 

Our concept was first validated using Postman integrating with 1R server. An API service was then developed acting as the middleware between the frontend and 1R server. The frontend sends request data based on the business model.
NX Monorepos and a microfrontend framework were implemented, allowing developers to maintain modular and manage code more easily. Besides NE:One Server and NE:One Play, we also utilized Vue.js and .NET Core to develop our solution, update shipment milestones and notify users.

## Challenges we ran into

**Challenge #1  ONE Record Challenge**

Develop a comprehensive tool to support implementation or a system that harnesses IATA's ONE Record standard to drive its adoption. 

**Challenge #2  CargoiQ Challenge**

Use Cargo iQ’s concept of Flight Route map and visualize the exchanges between airlines and GHA Warehouse or RAMP for export or import activities using ONE Record standard.  

**Challenge #3  Open Challenge**

Active notification for flight discrepancy. 

## What are you proud of?

- Comprehensive dashboard of flight & shipment info to facilitate grounding handling  

- Mobile app utilization for handiness 

- Multi-functional platform 

- Proactive solution approach for flight irregularity 

- Our customer-centric mindset 

## What is next step for EzyHandle?

**Automated Rebooking Feature**

- By analyzing current flight schedules, standard handling procedure and available flight alternatives, EzyHandle can provide automated rebooking function to minimize disruptions. 

**Issue Tracking**

- A centralized platform for airlines and GHA to report and trace cargo irregularities.  

**Integration with existing system**

- To facilitate adoption of the solution by integrating with Airline and GHA’s existing system. 
