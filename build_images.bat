@echo off

echo Building Docker images....

:: Set the context to the src directory
cd src

:: Building BookingOrchestratorService
echo Building image for booking_orchestrator_service...
docker build -t booking_orchestrator_service:latest -f .\BookingOrchestratorService\Dockerfile .

:: Building FlyghtService
echo Building image for flyght_service...
docker build -t flyght_service:latest -f .\FlyghtService\Dockerfile .

:: Building HotelService
echo Building image for hotel_service...
docker build -t hotel_service:latest -f .\HotelService\Dockerfile .

:: Building TransferService
echo Building image for transfer_service...
docker build -t transfer_service:latest -f .\TransferService\Dockerfile .

:: Building UiApiService
echo Building image for uiapi_service...
docker build -t uiapi_service:latest -f .\UiApiService\Dockerfile .

:: Return to the initial directory
cd ..

echo All services built successfully.
