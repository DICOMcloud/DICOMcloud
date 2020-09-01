sudo docker build --rm -t dicom-cloud-api-test ../ -f ../Development.Dockerfile
sudo docker run --rm -it -p 5021:80 dicom-cloud-api-test