# LS_Explorer
 Exploring N-dimensional latent spaces generated by neural autoencoders

 Exploratory Semi-Supervised Machine Learning (ESS-ML) is composed of following pipeline to analyze data as its latent (or hidden) dimensions revealing relationships useful to understanding the data. 
![](docs/images/ESS-ML-Pipeline.png)

This repository contains the code for Module 6 - Exploring LS Patterns, which taking data from the LS Data Server and preparing it for analysis in the next model - Applying LS Insights to a specific business problem by domain experts. 

## Input Data

+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
| Column0 | Variable                                                          | DataType | Shape        | Comments                                                         | Column5  |
+=========+===================================================================+==========+==============+==================================================================+==========+
|         |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
| Plot    |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | plotSpace                                                         | string   |              | name of dataset containing Latent Space data                     |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | plotShape                                                         | int, int | (1, 1)       | number of samples n by number of dimensions d                    |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | plotBase                                                          | int, int | (1, 1)       | two of d dimensions forming horizontal XZ axes                   |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | plotPole                                                          | int      |              | third of d dimensions forming vertical Y axis                    |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | plotScale                                                         | float    |              | muliplier to pointPos and pointStd values when ploting space     |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
| Point   |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | pointPos                                                          | float    | (n, d)       | position in d dimensional space of each of n samples             |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | pointStd                                                          | float    | (n, d)       | std deviation in the position for each point position            | optional |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | pointImage                                                        | int8     | (n, c, h, w) | image as c channels, h pixels high, w pixels wide for each point | optional |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
| Cluster |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | clusterName                                                       | string   |              | name of the cluster from Target, Feature, PCA, DBSCAN, manual    |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | clusterIndex                                                      | int      | n            | index into pointPos giving cluster no (with -1 as no cluster)    |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | clusterLabel                                                      | string   | k            | name for each unique clusterIndex value                          | optional |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | clusterVolume                                                     | float    | (d, 2)       | max/min for each m dimension                                     |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
| Methods |                                                                   |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | openPlot(plotSpace,                                               |          |              | returns plotID                                                   |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | closePlot(plotID)                                                 |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | setPlotBase(plotID, xAxisDim, zAxisDim)                           |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | setPlotPole(plotID, yAxisDim)                                     |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | contrastPlot(plotID, newAxisDim)                                  |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | createPoints(plotID, pointPrefab,                                 |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+
|         | showCluster(clusterName, clusterLabel=None,     returns clusterID |          |              |                                                                  |          |
+---------+-------------------------------------------------------------------+----------+--------------+------------------------------------------------------------------+----------+

