// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Modules
{
    /// <summary>
    /// Comfortable browser for modules
    /// </summary>
    public class ModulesBrowser
    {
        private readonly IApiRequestHandler _apiRequestHandler;
        private readonly ILogger _logger;

        /// <summary>
        /// Comfortable browser for modules
        /// </summary>
        /// <param name="apiRequestHandler">Api Request Handler to be used by the browser</param>
        /// <param name="logger">Logger for the browser</param>
        public ModulesBrowser(IApiRequestHandler apiRequestHandler, ILogger logger = null)
        {
            _apiRequestHandler = apiRequestHandler;
            _logger = logger;
        }

        /// <summary>
        /// Browse all Modules starting from the root node recursively
        /// </summary>
        /// <param name="cancellationToken">cancellation token for the operation</param>
        /// <returns>All the modules that have been browsed</returns>
        public async Task<List<Module>> BrowseAllModulesAsync(CancellationToken cancellationToken = default)
        {
            var started = DateTime.Now;
            _logger?.LogDebug($"Start browsing modules at: {started}");
            var response = await _apiRequestHandler.ModulesBrowseAsync(mode: ModulesBrowseMode.Children, cancellationToken: cancellationToken);
            var root = response.Result;
            foreach (Module node in response.Result.Nodes)
            {
                _logger?.LogTrace($"start browsing through: {node.Name}");
                await RecursiveBrowseAsync(node, cancellationToken);
            }
            var ended = DateTime.Now;
            _logger?.LogDebug($"Done browsing modules at: {ended} => took: {ended - started}");
            return root.Nodes;
        }

        /// <summary>
        /// Recursively browse through all availabe nodes starting from the given node
        /// </summary>
        /// <param name="node">node to start from</param>
        /// <param name="cancellationToken">cancellation token for the operation</param>
        /// <returns></returns>
        public async Task RecursiveBrowseAsync(Module node, CancellationToken cancellationToken = default)
        {
            if (node.HasChildren)
            {
                var parentList = node.Parent.Append(node.Hwid).ToList();
                if (node.Children.Count == 0)
                {
                    var response = await _apiRequestHandler.ModulesBrowseAsync(mode: ModulesBrowseMode.Children, hwid: node.Hwid, cancellationToken: cancellationToken);
                    var children = response.Result;
                    node.Children = children.Nodes.First().Children;
                    _logger?.LogTrace($"start browsing through: {node.Children.Count} child nodes of: {node.Name}");
                    foreach (Module childNode in node.Children)
                    {
                        childNode.Parent = parentList;
                        await RecursiveBrowseAsync(childNode, cancellationToken);
                    }
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        if (child.Parent == null || child.Parent.Count == 0)
                        {
                            child.Parent = parentList;
                        }
                        _logger?.LogTrace($"start browsing through child: {child.Name} - {child.Hwid} of: {node.Name} - {node.Hwid}");
                        await RecursiveBrowseAsync(child, cancellationToken);
                    }
                }
            }
            else
            {
                _logger?.LogTrace($"Nothing to do for node: {node.Name} since it has no children.");
            }
        }
    }
}
