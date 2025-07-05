using System.Threading.Tasks;
using UnityEngine;
using Opc.Ua;
using Opc.Ua.Client;
using System;

public class OPCUAClientTest : MonoBehaviour
{
    private Session session;
    private string serverUrl = "opc.tcp://localhost:4840/server/";
    private NodeId NODE_R1d_Joi1;
    private NodeId NODE_R1d_Joi2;  // New NodeId for Joint 2
    private double Rd_Joi1 = -15.0;
    private double Rd_Joi2 = 39.0; // Joint 2 default value

    async void Start()
    {
        Debug.Log("Initializing OPC UA client...");

        // Initialize the OPC UA client
        await InitializeOPCUAClient();

        // Once the client is initialized, start the polling loop
        if (session != null && session.Connected)
        {
            await PollingLoop();
        }
    }

    private async Task InitializeOPCUAClient()
    {
        try
        {
            Debug.Log("Connecting to OPC UA server...");

            var endpointDescription = CoreClientUtils.SelectEndpoint(serverUrl, false);
            var config = new ApplicationConfiguration
            {
                ApplicationName = "Unity OPC UA Client Test",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };

            // Set up endpoint configuration
            EndpointConfiguration endpointConfig = EndpointConfiguration.Create(config);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfig);

            session = await Session.Create(config, endpoint, false, "Unity OPC UA Client Session", 60000, null, null);

            Debug.Log($"Connected to OPC UA server at {serverUrl}");

            // Get the namespace index for 'http://example.com/robot1'
            int namespaceIndex = session.NamespaceUris.GetIndex("http://example.com/robot1");
            Debug.Log($"Namespace index for 'http://example.com/robot1' is {namespaceIndex}");

            // Initialize NodeId for Joint 1 and Joint 2
            NODE_R1d_Joi1 = new NodeId("R1d_Joi1", (ushort)namespaceIndex);
            NODE_R1d_Joi2 = new NodeId("R1d_Joi2", (ushort)namespaceIndex);
            Debug.Log($"Initialized NODE_R1d_Joi1 with NodeId: {NODE_R1d_Joi1}");
            Debug.Log($"Initialized NODE_R1d_Joi2 with NodeId: {NODE_R1d_Joi2}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while initializing OPC UA client or connecting to the server: " + ex.Message);
        }
    }

    private async Task PollingLoop()
    {
        Debug.Log("Entering Main Polling Loop");

        try
        {
            while (true)
            {
                Debug.Log("Polling OPC UA server...");

                await Task.Delay(1000); // 1-second delay

                // Read Joint1 and Joint2 data from the server
                try
                {
                    if (session != null && session.Connected)
                    {
                        DataValue joint1Value = await session.ReadValueAsync(NODE_R1d_Joi1);
                        DataValue joint2Value = await session.ReadValueAsync(NODE_R1d_Joi2);

                        // Check if the read operation was successful for Joint 1
                        if (StatusCode.IsGood(joint1Value.StatusCode))
                        {
                            Rd_Joi1 = (double)joint1Value.Value;
                            Debug.Log($"Read Joint 1 value: {Rd_Joi1}");
                        }
                        else
                        {
                            Debug.LogError($"Failed to read Joint 1 value: {joint1Value.StatusCode}");
                        }

                        // Check if the read operation was successful for Joint 2
                        if (StatusCode.IsGood(joint2Value.StatusCode))
                        {
                            Rd_Joi2 = (double)joint2Value.Value;
                            Debug.Log($"Read Joint 2 value: {Rd_Joi2}");
                        }
                        else
                        {
                            Debug.LogError($"Failed to read Joint 2 value: {joint2Value.StatusCode}");
                        }
                    }
                    else
                    {
                        Debug.LogError("Session is not connected.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error reading values: " + ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in Polling Loop: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        // Clean up the OPC UA session when the script is destroyed
        if (session != null)
        {
            session.Close();
            session.Dispose();
            Debug.Log("OPC UA client session cleaned up.");
        }
    }
}
