#!/bin/bash

echo "Stopping SentinelX services..."
docker-compose down

echo "Cleaning up data..."
rm -rf data/
rm -rf logs/

echo "Cleanup complete!"
