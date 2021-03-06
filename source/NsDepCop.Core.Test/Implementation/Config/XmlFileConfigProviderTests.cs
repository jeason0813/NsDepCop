﻿using System.Threading;
using Codartis.NsDepCop.Core.Implementation.Config;
using Codartis.NsDepCop.Core.Interface.Config;
using FluentAssertions;
using Xunit;

namespace Codartis.NsDepCop.Core.Test.Implementation.Config
{
    public class XmlFileConfigProviderTests : XmlFileConfigTestBase
    {
        [Fact]
        public void Properties_ConfigNotFound()
        {
            var path = GetFilePathInTestClassFolder("NonExisting.nsdepcop");
            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.NoConfig);
            configProvider.ConfigException.Should().BeNull();
            configProvider.Config.Should().BeNull();
        }

        [Fact]
        public void Properties_ConfigEnabled()
        {
            var path = GetFilePathInTestClassFolder("Enabled.nsdepcop");
            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);
            configProvider.ConfigException.Should().BeNull();
            configProvider.Config.Should().NotBeNull();
        }

        [Fact]
        public void Properties_ConfigDisabled()
        {
            var path = GetFilePathInTestClassFolder("Disabled.nsdepcop");
            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Disabled);
            configProvider.ConfigException.Should().BeNull();
            configProvider.Config.Should().BeNull();
        }

        [Fact]
        public void Properties_ConfigError()
        {
            var path = GetFilePathInTestClassFolder("Erronous.nsdepcop");
            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.ConfigError);
            configProvider.ConfigException.Should().NotBeNull();
            configProvider.Config.Should().BeNull();
        }

        [Fact]
        public void RefreshConfig_Unchanged()
        {
            var path = GetFilePathInTestClassFolder("Enabled.nsdepcop");

            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);
            var savedConfig = configProvider.Config;

            configProvider.RefreshConfig();
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);
            configProvider.Config.Should().Be(savedConfig);
        }

        [Fact]
        public void RefreshConfig_EnabledToDisabled()
        {
            var path = GetFilePathInTestClassFolder("RefreshConfig_EnabledToDisabled.nsdepcop");

            SetAttribute(path, "IsEnabled", "true");

            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);

            Thread.Sleep(10);
            SetAttribute(path, "IsEnabled", "false");

            configProvider.RefreshConfig();
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Disabled);
        }

        [Fact]
        public void RefreshConfig_EnabledToConfigError()
        {
            var path = GetFilePathInTestClassFolder("RefreshConfig_EnabledToConfigError.nsdepcop");

            SetAttribute(path, "IsEnabled", "true");

            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);

            Thread.Sleep(10);
            SetAttribute(path, "IsEnabled", "maybe");

            configProvider.RefreshConfig();
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.ConfigError);
        }

        [Fact]
        public void RefreshConfig_NoConfigToEnabled()
        {
            var path = GetFilePathInTestClassFolder("RefreshConfig_NoConfigToEnabled.nsdepcop");

            Delete(path);

            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.NoConfig);

            CreateConfigFile(path, "true");

            configProvider.RefreshConfig();
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);
        }

        [Fact]
        public void RefreshConfig_EnabledToNoConfig()
        {
            var path = GetFilePathInTestClassFolder("RefreshConfig_EnabledToNoConfig.nsdepcop");

            Delete(path);
            CreateConfigFile(path, "true");

            var configProvider = CreateConfigProvider(path);
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.Enabled);

            Delete(path);

            configProvider.RefreshConfig();
            configProvider.ConfigState.Should().Be(AnalyzerConfigState.NoConfig);
        }

        private static XmlFileConfigProvider CreateConfigProvider(string path)
        {
            return new XmlFileConfigProvider(path, traceMessageHandler: null);
        }
    }
}

