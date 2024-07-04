Vagrant.configure("2") do |config|
    config.vm.box = "gusztavvargadr/windows-11"
    config.vm.provider "virtualbox" do |vb|
        vb.gui = true
    end
    config.vm.synced_folder ".", "C:\\vagrant"
end
