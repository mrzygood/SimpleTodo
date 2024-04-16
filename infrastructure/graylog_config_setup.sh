#!/bin/bash

# Configuration
API_BASE_URL="http://localhost:9000/api" # API base url for graylog instance run locally
API_AUTH_HEADER='Authorization: Basic YWRtaW46YWRtaW4=' # encoded in base64 login and password

# Variables populated dynamically
index_set_id=""

printf '%s\n' "-> Create index set"

index_json_response=$(curl -sS --location "${API_BASE_URL}/system/indices/index_sets" \
    --header 'X-Requested-By: SetupBashScript' \
    --header 'Content-Type: application/json' \
    --header "$API_AUTH_HEADER" \
    --data '{
      "title": "simple_todo_index",
      "index_prefix": "todo_",
      "description": "Example todo app index set",
      "rotation_strategy_class": "org.graylog2.indexer.rotation.strategies.TimeBasedRotationStrategy",
      "rotation_strategy": {
          "type": "org.graylog2.indexer.rotation.strategies.TimeBasedRotationStrategyConfig",
          "rotation_period": "P1D"
      },
      "retention_strategy_class": "org.graylog2.indexer.retention.strategies.DeletionRetentionStrategy",
      "retention_strategy": {
          "type": "org.graylog2.indexer.retention.strategies.DeletionRetentionStrategyConfig",
          "max_number_of_indices": 2
      },
      "index_analyzer": "standard",
      "field_type_refresh_interval": "PT5S",
      "shards": 1,
      "index_optimization_max_num_segments": 1,
      "index_optimization_disabled": false,
      "writable": true
    }')
    
if [ $? -eq 0 ]; then
    index_set_id=$(echo "$index_json_response" | grep -o '"id": *"[a-zA-Z0-9]*"' | awk -F ':' '{print $2}')        
    echo "Graylog index ID: ${index_set_id}"
else
    echo "Error: Index creation request failed"
fi

printf '%s\n' "-> Add UDP input"

input_json_response=$(curl -sS --location "${API_BASE_URL}/system/inputs" \
                      --header 'X-Requested-By: SetupBashScript' \
                      --header 'Content-Type: application/json' \
                      --header "$API_AUTH_HEADER" \
                      --data '{
                          "type": "org.graylog2.inputs.gelf.udp.GELFUDPInput",
                          "title": "simple_todo-udp",
                          "global": true,
                          "configuration": {
                              "charset_name": "UTF-8",
                              "number_worker_threads": 12,
                              "recv_buffer_size": 262144,
                              "bind_address": "0.0.0.0",
                              "port": 12201,
                              "decompress_size_limit": 8388608
                          }
                      }')

if [ $? -eq 0 ]; then
    graylog_input_id=$(echo "$input_json_response" | grep -o '"id": *"[a-zA-Z0-9]*"' | awk -F ':' '{print $2}')        
    echo "Graylog input ID: ${graylog_input_id}"
else
    echo "Error: Input creation request failed"
fi

printf '%s\n' "-> Add a stream"

stream_json_response=$(curl -sS --location "${API_BASE_URL}/streams" \
                       --header 'X-Requested-By: SetupBashScript' \
                       --header 'Content-Type: application/json' \
                       --header "$API_AUTH_HEADER" \
                       --data "{
                           \"index_set_id\": ${index_set_id},
                           \"remove_matches_from_default_stream\": true,
                           \"description\": \"With all messages\",
                           \"title\": \"simple_todo-core\",
                           \"matching_type\": \"AND\",
                           \"rules\": [
                               {
                                   \"field\": \"ApplicationName\",
                                   \"description\": \"Logs from todo app\",
                                   \"type\": 1,
                                   \"inverted\": false,
                                   \"value\": \"SimpleTodo\"
                               }
                           ]
                       }")

if [ $? -eq 0 ]; then
    stream_id=$(echo "$stream_json_response" | grep -o '"stream_id": *"[a-zA-Z0-9]*"' | awk -F ':' '{print $2}')        
    echo "Graylog stream ID: ${stream_id}"
else
    echo "Error: Stream creation request failed"
fi

printf '%s\n' "-> Start the stream"

raw_stream_id=${stream_id//\"/}

curl -sS --location --request POST "${API_BASE_URL}/streams/${raw_stream_id}/resume" \
     --header 'X-Requested-By: SetupScript' --header "$API_AUTH_HEADER"

if [ $? -eq 0 ]; then
    echo "Graylog stream with ID: ${stream_id} was started"
else
    echo "Error: Stream starting request failed"
fi
