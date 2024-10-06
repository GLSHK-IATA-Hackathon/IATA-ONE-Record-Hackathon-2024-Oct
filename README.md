## Inspiration

The operation of the air cargo industry lacks transparency in service delivery. 

Key Problems to Address: 

1. Communication Breakdowns: During disruptive events, ineffective communication can result in discrepancies that adversely affect shipment performance. 

2. Increased Discrepancies: Poor coordination and communication can lead to a rising number of discrepancies, undermining operational efficiency and jeopardizing timely deliveries. 

3. Lack of Performance Assessment Tools: Currently, there is no digital tool to compare expected milestone times with actual performance, making it difficult to assess GHA Service Level Agreement (SLA) performance.  

## Solution and what it does 

1. Development of a Comprehensive Communication System Includes a mobile app and website tailored for Ground Handling Agents (GHAs) and airlines to enhance communication, streamline resource reallocation, and proactively manage potential delays. 

2. Centralized Shipment Information Retrieves flight info from a centralized repository (1R). Incorporates the Cargo IQ concept to compare planned and actual event times. 

3. Real-Time Updates Frontline staff can use the EzyHandle mobile app to update shipment milestones in real-time. 

4. Flight Movement Tracking Integrates flight movement data from airlines and GHA to identify delays promptly. 

5. Management of Delayed Milestones and Proactive Handling Suggestions  Highlights delayed milestones and recommends workaround 

## How did we build it? 

EzyHandle will retrieve shipment flight information from a centralized repository (1R) and incorporate the Cargo IQ concept, which compares planned event times with actual event times. Frontline operations staff can use the EzyHandle mobile app to update shipment milestones in real time.  

EzyHandle will also receive flight movement data from airlines to identify delays. By highlighting delayed milestones and recommending handling procedures specific to each airline, the system can significantly improve operational efficiency. 

Besides NE:One Server and NE:One Play, we utilized Vue.js and .NET Core to develop our solution, update shipment milestones and notify users.

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

- Proactive notification to GHA 

- Our customer-centric mindset 

## What is next step for EzyHandle?

1. Automated Rebooking Feature 

- By analyzing current flight schedules, standard handling procedure and available flight alternatives, EzyHandle can provide automated rebooking function to minimize disruptions. 

2. Issue Tracking 

- A centralized platform for airlines and GHA to report and trace cargo irregularities.  

3. Integration with existing system 

- To facilitate adoption of the solution by integrating with Airline and GHA’s existing system. 
