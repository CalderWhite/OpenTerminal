package org.openterminal.backend;

import com.corundumstudio.socketio.Configuration;
import org.apache.log4j.BasicConfigurator;
import org.apache.log4j.varia.NullAppender;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

public class Server {
    public static void main(String[] args) throws IOException {
        // Configure logging:
        // org.apache.log4j.BasicConfigurator.configure(); // verbose
        BasicConfigurator.configure(new NullAppender()); // silent, doesn't affect data logging

        // SocketIO config
        Configuration config = new Configuration();
        config.setHostname("localhost");
        config.setPort(2000);

        // Create the SocketIO server.
        SocketIO socketIOServer = new SocketIO(config);
        socketIOServer.enableDataLogging();
        socketIOServer.start();
        System.out.println("Running on "+config.getHostname()+" port "+config.getPort()+".");

        // Command processing.
        System.out.println("Type \"help\" for command-line help.");

        String cmd;
        boolean running = true;
        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));

        while (running) {
            cmd = reader.readLine();

            switch (cmd) {
                case "exit" : {
                    System.out.println("Shutting down server...");
                    running = false;
                    break;
                }
                case "help" : {
                    System.out.println(
                            "-----------------------------------------------\n" +
                            "exit\t\t\t\tShut down the server.\n" +
                            "help\t\t\t\tPrint this message.\n" +
                            "disableLogging\t\tDisable trade data logging.\n" +
                            "enableLogging\t\tEnable trade data logging.\n" +
                            "-----------------------------------------------"
                    );
                    break;
                }
                case "disableLogging" : {
                    socketIOServer.disableDataLogging();
                    System.out.println("Data logging disabled.");
                    break;
                }
                case "enableLogging" : {
                    socketIOServer.enableDataLogging();
                    System.out.println("Data logging enabled.");
                    break;
                }
                default : {
                    System.out.println("Command unrecognized.");
                    break;
                }
            }
        }

        // Close server.
        socketIOServer.stop();
    }
}
