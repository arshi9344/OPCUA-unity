import logging
import asyncio
from asyncua import ua, Server

async def main():
    _logger = logging.getLogger('asyncua')
    logging.basicConfig(level=logging.DEBUG)

    # Setup server
    server = Server()
    await server.init()
    server.set_endpoint('opc.tcp://localhost:4840/server/')
    server.set_server_name('Test OPC UA Server')

    # Set security policies (anonymous access)
    server.set_security_policy([ua.SecurityPolicyType.NoSecurity])
    server.set_security_IDs(["Anonymous"])

    # Register a unique namespace
    uri = 'http://example.com/robot1'
    idx = await server.register_namespace(uri)
    _logger.info(f'Registered namespace {uri} with index {idx}')

    # Add "robot1" object under the standard 'Objects' node
    objects_node = server.nodes.objects
    robot1 = await objects_node.add_object(idx, 'robot1')
    _logger.info(f'Added object "robot1" with NodeId: {robot1.nodeid}')

    # Define string-based NodeIds for the variables
    R1d_Joi1 = await robot1.add_variable(ua.NodeId('R1d_Joi1', idx), 'R1d_Joi1', ua.uatypes.Double(-15.0))
    await R1d_Joi1.set_writable()
    _logger.info(f'Added variable "R1d_Joi1" with NodeId: {R1d_Joi1.nodeid}')

    R1d_Joi2 = await robot1.add_variable(ua.NodeId('R1d_Joi2', idx), 'R1d_Joi2', ua.uatypes.Double(39.0))
    await R1d_Joi2.set_writable()
    _logger.info(f'Added variable "R1d_Joi2" with NodeId: {R1d_Joi2.nodeid}')

    # Set Joint 2 value to +45
    await R1d_Joi2.write_value(ua.uatypes.Double(45.0)) 
    _logger.info(f'Updated "R1d_Joi2" value to 45.0')

    _logger.info('All nodes initialized successfully.')

    # Start the server
    _logger.info('Starting server!')
    async with server:
        while True:
            await asyncio.sleep(0.01)
            _logger.debug('Server running, waiting for client connections...')

if __name__ == '__main__':
    asyncio.run(main())
